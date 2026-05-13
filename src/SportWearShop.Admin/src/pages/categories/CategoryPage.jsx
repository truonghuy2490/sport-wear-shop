import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";

import { getRootCategories, deleteCategory } from "../../api/categoryApi";

import PageHeader from "../../components/common/PageHeader";
import SearchBox from "../../components/common/SearchBox";
import StatusBadge from "../../components/common/StatusBadge";

import { showToast } from "../../redux/toast/toastSlice";

import { Link, useNavigate } from "react-router-dom";

function CategoryPage() {
    const [categories, setCategories] = useState([]);
    const [searchKeyword, setSearchKeyword] = useState("");
    const [isLoading, setIsLoading] = useState(true);
    const navigate = useNavigate();

    const dispatch = useDispatch();

    useEffect(() => {
        loadCategories();
    }, []);

    async function loadCategories() {
        try {
            setIsLoading(true);

            const data = await getRootCategories();

            setCategories(data);
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to load categories."
                })
            );
        } finally {
            setIsLoading(false);
        }
    }

    async function handleDeleteCategory(categoryId) {
        const confirmed = window.confirm(
            "Are you sure you want to delete this category?"
        );

        if (!confirmed) return;

        try {
            await deleteCategory(categoryId);

            setCategories((prevCategories) =>
                prevCategories.filter(
                    (category) => category.categoryId !== categoryId
                )
            );

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Category deleted successfully."
                })
            );
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to delete category."
                })
            );
        }
    }

    function getCategoryLevel(category) {
        let level = 0;
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

    const filteredCategories = categories.filter(
        (category) =>
            category.categoryName
                .toLowerCase()
                .includes(searchKeyword.toLowerCase()) ||
            category.categoryCode
                .toLowerCase()
                .includes(searchKeyword.toLowerCase())
    );

    const sortedCategories = [...filteredCategories].sort(
        (a, b) => a.sortOrder - b.sortOrder
    );

    return (
        <div>
            <PageHeader
                title="Categories"
                description="Manage category hierarchy and structure."
                actionText="+ Add Category"
                onActionClick={() => navigate("/categories/create")}
            />

            <div className="card border-0 shadow-sm">
                <div className="card-body">

                    <div className="d-flex justify-content-between align-items-center mb-3">
                        <h5 className="mb-0 fw-semibold">
                            Category List
                        </h5>

                        <SearchBox
                            value={searchKeyword}
                            onChange={setSearchKeyword}
                            placeholder="Search category..."
                        />
                    </div>

                    {isLoading ? (
                        <div className="text-center py-5 text-muted">
                            Loading categories...
                        </div>
                    ) : (
                        <div className="table-responsive">
                            <table className="table table-hover align-middle">
                                <thead className="table-light">
                                    <tr>
                                        <th>#</th>
                                        <th>Category Name</th>
                                        <th>Code</th>
                                        <th>Description</th>
                                        <th>Sort Order</th>
                                        <th>Status</th>
                                        <th className="text-end">
                                            Actions
                                        </th>
                                    </tr>
                                </thead>

                                <tbody>
                                    {sortedCategories.length === 0 ? (
                                        <tr>
                                            <td
                                                colSpan="7"
                                                className="text-center text-muted py-4"
                                            >
                                                No categories found.
                                            </td>
                                        </tr>
                                    ) : (
                                        sortedCategories.map((category, index) => {
                                            const level = getCategoryLevel(category);

                                            return (
                                                <tr key={category.categoryId}>
                                                    <td>
                                                        {index + 1}
                                                    </td>

                                                    <td
                                                        className="fw-medium"
                                                        style={{
                                                            paddingLeft: `${level * 32 + 16}px`
                                                        }}
                                                    >
                                                        {level > 0 && (
                                                            <span className="text-muted me-2">
                                                                └─
                                                            </span>
                                                        )}

                                                        {category.categoryName}
                                                    </td>

                                                    <td>
                                                        {category.categoryCode}
                                                    </td>

                                                    <td>
                                                        {category.description || "-"}
                                                    </td>

                                                    <td>
                                                        {category.sortOrder}
                                                    </td>

                                                    <td>
                                                        <StatusBadge
                                                            isActive={category.isActive}
                                                        />
                                                    </td>

                                                    <td className="text-end">
                                                        <Link
                                                            to={`/categories/${category.categoryId}`}
                                                            className="btn btn-sm btn-outline-primary me-2"
                                                        >
                                                            View
                                                        </Link>

                                                        <Link
                                                            to={`/categories/${category.categoryId}/edit`}
                                                            className="btn btn-sm btn-outline-dark me-2"
                                                        >
                                                            Edit
                                                        </Link>

                                                        <button
                                                            type="button"
                                                            className="btn btn-sm btn-outline-danger"
                                                            onClick={() =>
                                                                handleDeleteCategory(
                                                                    category.categoryId
                                                                )
                                                            }
                                                        >
                                                            Delete
                                                        </button>
                                                    </td>
                                                </tr>
                                            );
                                        })
                                    )}
                                </tbody>
                            </table>
                        </div>
                    )}

                </div>
            </div>
        </div>
    );
}

export default CategoryPage;