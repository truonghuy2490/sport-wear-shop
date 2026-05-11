import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useDispatch } from "react-redux";

import {
    getCategories,
    getCategoryById,
    updateCategory
} from "../../api/categoryApi";

import { showToast } from "../../redux/toast/toastSlice";

function CategoryEditPage() {
    const { categoryId } = useParams();

    const navigate = useNavigate();
    const dispatch = useDispatch();

    const [categories, setCategories] = useState([]);

    const [formData, setFormData] = useState({
        parentCategoryId: "",
        categoryName: "",
        categoryCode: "",
        description: "",
        sortOrder: 0,
        isActive: true
    });

    const [isLoading, setIsLoading] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        loadData();
    }, [categoryId]);

    async function loadData() {
        try {
            setIsLoading(true);

            const [categoryData, categoryList] = await Promise.all([
                getCategoryById(categoryId),
                getCategories()
            ]);

            setCategories(categoryList);

            setFormData({
                parentCategoryId: categoryData.parentCategoryId || "",
                categoryName: categoryData.categoryName || "",
                categoryCode: categoryData.categoryCode || "",
                description: categoryData.description || "",
                sortOrder: categoryData.sortOrder || 0,
                isActive: categoryData.isActive
            });
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to load category form."
                })
            );
        } finally {
            setIsLoading(false);
        }
    }

    function getCategoryLevel(category) {
        let level = 1;
        let parentId = category.parentCategoryId;

        while (parentId) {
            const parent = categories.find(
                (item) => item.categoryId === parentId
            );

            if (!parent) break;

            level += 1;
            parentId = parent.parentCategoryId;
        }

        return level;
    }

    const parentOptions = categories.filter(
        (category) =>
            category.categoryId !== Number(categoryId) &&
            getCategoryLevel(category) < 3
    );

    function handleChange(e) {
        const { name, value, type, checked } = e.target;

        setFormData((prev) => ({
            ...prev,
            [name]: type === "checkbox" ? checked : value
        }));
    }

    async function handleSubmit(e) {
        e.preventDefault();

        const request = {
            ...formData,
            parentCategoryId: formData.parentCategoryId
                ? Number(formData.parentCategoryId)
                : null,
            sortOrder: Number(formData.sortOrder)
        };

        try {
            setIsSubmitting(true);

            await updateCategory(categoryId, request);

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Category updated successfully."
                })
            );

            navigate(`/categories/${categoryId}`);
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to update category."
                })
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    if (isLoading) {
        return (
            <div className="text-center py-5 text-muted">
                Loading category form...
            </div>
        );
    }

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">
                        Edit Category
                    </h3>

                    <p className="text-muted mb-0">
                        Update category information and hierarchy.
                    </p>
                </div>

                <Link
                    to={`/categories/${categoryId}`}
                    className="btn btn-outline-secondary"
                >
                    Back
                </Link>
            </div>

            <div className="card border-0 shadow-sm">
                <div className="card-body p-4">
                    <form onSubmit={handleSubmit}>
                        <div className="row g-3">
                            <div className="col-md-6">
                                <label className="form-label fw-medium">
                                    Category Name
                                </label>

                                <input
                                    type="text"
                                    name="categoryName"
                                    className="form-control"
                                    value={formData.categoryName}
                                    onChange={handleChange}
                                    required
                                />
                            </div>

                            <div className="col-md-6">
                                <label className="form-label fw-medium">
                                    Category Code
                                </label>

                                <input
                                    type="text"
                                    name="categoryCode"
                                    className="form-control"
                                    value={formData.categoryCode}
                                    onChange={handleChange}
                                    required
                                />
                            </div>

                            <div className="col-md-6">
                                <label className="form-label fw-medium">
                                    Parent Category
                                </label>

                                <select
                                    name="parentCategoryId"
                                    className="form-select"
                                    value={formData.parentCategoryId}
                                    onChange={handleChange}
                                >
                                    <option value="">
                                        Root Category
                                    </option>

                                    {parentOptions.map((category) => (
                                        <option
                                            key={category.categoryId}
                                            value={category.categoryId}
                                        >
                                            {"— ".repeat(getCategoryLevel(category) - 1)}
                                            {category.categoryName}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="col-md-6">
                                <label className="form-label fw-medium">
                                    Sort Order
                                </label>

                                <input
                                    type="number"
                                    name="sortOrder"
                                    className="form-control"
                                    value={formData.sortOrder}
                                    onChange={handleChange}
                                />
                            </div>

                            <div className="col-md-12">
                                <label className="form-label fw-medium">
                                    Description
                                </label>

                                <textarea
                                    name="description"
                                    className="form-control"
                                    rows="3"
                                    value={formData.description}
                                    onChange={handleChange}
                                />
                            </div>

                            <div className="col-md-12">
                                <div className="form-check">
                                    <input
                                        type="checkbox"
                                        name="isActive"
                                        id="isActive"
                                        className="form-check-input"
                                        checked={formData.isActive}
                                        onChange={handleChange}
                                    />

                                    <label
                                        htmlFor="isActive"
                                        className="form-check-label"
                                    >
                                        Active
                                    </label>
                                </div>
                            </div>
                        </div>

                        <div className="d-flex justify-content-end gap-2 mt-4">
                            <Link
                                to="/categories"
                                className="btn btn-outline-secondary"
                            >
                                Cancel
                            </Link>

                            <button
                                type="submit"
                                className="btn btn-dark"
                                disabled={isSubmitting}
                            >
                                {isSubmitting ? "Saving..." : "Save Changes"}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}

export default CategoryEditPage;