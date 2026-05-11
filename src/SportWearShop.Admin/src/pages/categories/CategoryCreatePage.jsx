import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";

import { createCategory, getCategories } from "../../api/categoryApi";
import { showToast } from "../../redux/toast/toastSlice";

function CategoryCreatePage() {
    const navigate = useNavigate();
    const dispatch = useDispatch();

    const [categories, setCategories] = useState([]);

    const [formData, setFormData] = useState({
        parentCategoryId: "",
        categoryName: "",
        categoryCode: "",
        description: "",
        sortOrder: 0
    });

    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        loadCategories();
    }, []);

    async function loadCategories() {
        try {
            const data = await getCategories();
            setCategories(data);
        } catch (error) {
            console.error("Failed to load categories:", error);
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
        (category) => getCategoryLevel(category) < 3
    );

    function handleChange(e) {
        const { name, value } = e.target;

        setFormData((prev) => ({
            ...prev,
            [name]: value
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

            const createdCategory = await createCategory(request);

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Category created successfully."
                })
            );

            navigate(`/categories/${createdCategory.categoryId}`);
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to create category."
                })
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">
                        Add Category
                    </h3>

                    <p className="text-muted mb-0">
                        Create a new category in the hierarchy.
                    </p>
                </div>

                <Link
                    to="/categories"
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

                                <div className="form-text">
                                    Maximum category level is 3.
                                </div>
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
                                {isSubmitting ? "Creating..." : "Create Category"}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}

export default CategoryCreatePage;