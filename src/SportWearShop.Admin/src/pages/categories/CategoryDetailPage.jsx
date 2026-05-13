import { useEffect, useState } from "react";
import { Link, useParams, useNavigate } from "react-router-dom";

import { getCategoryById, deleteCategory } from "../../api/categoryApi";
import StatusBadge from "../../components/common/StatusBadge";

function CategoryDetailPage() {
    const { categoryId } = useParams();
    const navigate = useNavigate();
    const [category, setCategory] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        loadCategoryDetail();
    }, [categoryId]);

    async function handleDeleteCategory(categoryId) {
        const confirmed = window.confirm(
            "Are you sure you want to delete this category?"
        );

        if (!confirmed) return;

        try {
            await deleteCategory(categoryId);
            await loadCategoryDetail();
        } catch (err) {
            console.error(err);
            alert(
                err?.response?.data?.message ||
                "Delete category failed. Please try again."
            );
        }
    }

    async function loadCategoryDetail() {
        try {
            setIsLoading(true);

            const data = await getCategoryById(categoryId);

            setCategory(data);
        } catch (error) {
            console.error("Failed to load category detail:", error);
        } finally {
            setIsLoading(false);
        }
    }

    if (isLoading) {
        return (
            <div className="text-center py-5 text-muted">
                Loading category detail...
            </div>
        );
    }

    if (!category) {
        return (
            <div className="text-center py-5 text-muted">
                Category not found.
            </div>
        );
    }

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">
                        Category Detail
                    </h3>

                    <p className="text-muted mb-0">
                        View category information and child categories.
                    </p>
                </div>

                <div>
                    <Link
                        to="/categories"
                        className="btn btn-outline-secondary me-2"
                    >
                        Back
                    </Link>

                    <Link
                        to={`/categories/${category.categoryId}/edit`}
                        className="btn btn-dark"
                    >
                        Edit Category
                    </Link>
                </div>
            </div>

            <div className="card border-0 shadow-sm mb-4">
                <div className="card-body p-4">
                    <h4 className="fw-bold mb-1">
                        {category.categoryName}
                    </h4>

                    <div className="text-muted mb-3">
                        {category.categoryCode}
                    </div>

                    <StatusBadge isActive={category.isActive} />

                    <hr />

                    <div className="row g-4">
                        <div className="col-md-6">
                            <div className="text-muted small">
                                Category ID
                            </div>
                            <div className="fw-medium">
                                {category.categoryId}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Parent Category ID
                            </div>
                            <div className="fw-medium">
                                {category.parentCategoryId || "Root Category"}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Sort Order
                            </div>
                            <div className="fw-medium">
                                {category.sortOrder}
                            </div>
                        </div>

                        <div className="col-md-12">
                            <div className="text-muted small">
                                Description
                            </div>
                            <div className="fw-medium">
                                {category.description || "-"}
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div className="card border-0 shadow-sm">
                <div className="card-body">
                    <h5 className="fw-semibold mb-3">
                        Child Categories
                    </h5>

                    {category.children?.length === 0 ? (
                        <div className="text-muted">
                            No child categories.
                        </div>
                    ) : (
                        <div className="table-responsive">
                            <table className="table table-hover align-middle">
                                <thead className="table-light">
                                    <tr>
                                        <th>#</th>
                                        <th>Name</th>
                                        <th>Code</th>
                                        <th>Sort Order</th>
                                        <th>Status</th>
                                        <th className="text-end">Actions</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    {category.children.map((child, index) => (
                                        <tr key={child.categoryId}>
                                            <td>{index + 1}</td>

                                            <td className="fw-medium">
                                                {child.categoryName}
                                            </td>

                                            <td>{child.categoryCode}</td>

                                            <td>{child.sortOrder}</td>

                                            <td>
                                                <StatusBadge isActive={child.isActive} />
                                            </td>

                                            <td className="text-end">
                                                <div className="btn-group">
                                                    <button
                                                        type="button"
                                                        className="btn btn-sm btn-outline-dark"
                                                        onClick={() =>
                                                            navigate(`/categories/${child.categoryId}/edit`)
                                                        }
                                                    >
                                                        Edit
                                                    </button>

                                                    <button
                                                        type="button"
                                                        className="btn btn-sm btn-outline-danger"
                                                        onClick={() =>
                                                            handleDeleteCategory(child.categoryId)
                                                        }
                                                    >
                                                        Delete
                                                    </button>
                                                </div>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default CategoryDetailPage;