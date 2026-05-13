import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { Link, useNavigate } from "react-router-dom";

import { getRootCategories, deleteCategory } from "../../api/categoryApi";

import PageHeader from "../../components/common/PageHeader";
import SearchBox from "../../components/common/SearchBox";
import StatusBadge from "../../components/common/StatusBadge";

import { showToast } from "../../redux/toast/toastSlice";

function CategoryPage() {
    const [categories, setCategories] = useState([]);
    const [searchKeyword, setSearchKeyword] = useState("");
    const [isLoading, setIsLoading] = useState(true);

    const [pageNumber, setPageNumber] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(1);

    const dispatch = useDispatch();
    const navigate = useNavigate();

    useEffect(() => {
        loadCategories();
    }, [pageNumber]);

    async function loadCategories() {
        try {
            setIsLoading(true);

            const data = await getRootCategories(pageNumber, pageSize);

            const categoryItems =
                data.items ||
                data.data ||
                data.results ||
                data.categories ||
                data;

            setCategories(Array.isArray(categoryItems) ? categoryItems : []);
            setTotalPages(data.totalPages || data.totalPage || 1);
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
            await loadCategories();

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

    const filteredCategories = categories.filter((category) => {
        const keyword = searchKeyword.toLowerCase();

        return (
            category.categoryName?.toLowerCase().includes(keyword) ||
            category.categoryCode?.toLowerCase().includes(keyword)
        );
    });

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
                        <h5 className="mb-0 fw-semibold">Category List</h5>

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
                        <>
                            <div className="table-responsive">
                                <table className="table table-hover align-middle">
                                    <thead className="table-light">
                                        <tr>
                                            <th>#</th>
                                            <th>Category Name</th>
                                            <th>Code</th>
                                            <th>Description</th>
                                            <th>Status</th>
                                            <th className="text-end">Actions</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        {filteredCategories.length === 0 ? (
                                            <tr>
                                                <td
                                                    colSpan="6"
                                                    className="text-center text-muted py-4"
                                                >
                                                    No categories found.
                                                </td>
                                            </tr>
                                        ) : (
                                            filteredCategories.map((category, index) => (
                                                <tr key={category.categoryId}>
                                                    <td>
                                                        {(pageNumber - 1) *
                                                            pageSize +
                                                            index +
                                                            1}
                                                    </td>

                                                    <td className="fw-medium">
                                                        {category.categoryName}
                                                    </td>

                                                    <td>{category.categoryCode}</td>

                                                    <td>
                                                        {category.description || "-"}
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

                                                        {category.isActive && (
                                                            <>
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
                                                            </>
                                                        )}
                                                    </td>
                                                </tr>
                                            ))
                                        )}
                                    </tbody>
                                </table>
                            </div>

                            <div className="d-flex justify-content-between align-items-center mt-3">
                                <div className="text-muted small">
                                    Page {pageNumber} of {totalPages}
                                </div>

                                <div>
                                    <button
                                        type="button"
                                        className="btn btn-sm btn-outline-secondary me-2"
                                        disabled={pageNumber <= 1}
                                        onClick={() =>
                                            setPageNumber((prev) => prev - 1)
                                        }
                                    >
                                        Previous
                                    </button>

                                    <button
                                        type="button"
                                        className="btn btn-sm btn-outline-secondary"
                                        disabled={pageNumber >= totalPages}
                                        onClick={() =>
                                            setPageNumber((prev) => prev + 1)
                                        }
                                    >
                                        Next
                                    </button>
                                </div>
                            </div>
                        </>
                    )}
                </div>
            </div>
        </div>
    );
}

export default CategoryPage;