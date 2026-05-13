import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
    getCategoryById,
    getCategoryChildren,
    createCategory,
    updateCategory,
    deleteCategory
} from "../../api/categoryApi";
import {
    createCategoryRequestModel,
    updateCategoryRequestModel
} from "../../models/categoryModel";

function CategoryEditPage() {
    const { categoryId } = useParams();
    const navigate = useNavigate();

    const [parentForm, setParentForm] = useState({
        ...updateCategoryRequestModel
    });

    const [childForms, setChildForms] = useState([]);

    const [isLoading, setIsLoading] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState("");

    useEffect(() => {
        loadCategory();
    }, [categoryId]);

    async function loadCategory() {
        try {
            setIsLoading(true);
            setError("");

            const parent = await getCategoryById(categoryId);
            const children = await getCategoryChildren(categoryId);

            setParentForm({
                parentCategoryId: parent.parentCategoryId,
                categoryName: parent.categoryName,
                categoryCode: parent.categoryCode,
                description: parent.description || "",
                sortOrder: parent.sortOrder,
                isActive: parent.isActive
            });

            setChildForms(
                children.map(child => ({
                    categoryId: child.categoryId,
                    parentCategoryId: child.parentCategoryId,
                    categoryName: child.categoryName,
                    categoryCode: child.categoryCode,
                    description: child.description || "",
                    sortOrder: child.sortOrder,
                    isActive: child.isActive,
                    isNew: false
                }))
            );
        } catch (err) {
            console.error(err);
            setError(
                err?.response?.data?.message ||
                "Load category failed. Please try again."
            );
        } finally {
            setIsLoading(false);
        }
    }

    function handleParentChange(field, value) {
        setParentForm(prev => ({
            ...prev,
            [field]: value
        }));
    }

    function handleChildChange(index, field, value) {
        setChildForms(prev => {
            const updated = [...prev];

            updated[index] = {
                ...updated[index],
                [field]: value
            };

            return updated;
        });
    }

    function handleAddChild() {
        setChildForms(prev => [
            ...prev,
            {
                ...createCategoryRequestModel,
                parentCategoryId: Number(categoryId),
                sortOrder: prev.length + 1,
                isActive: true,
                isNew: true
            }
        ]);
    }

    async function handleRemoveChild(index) {
        const child = childForms[index];

        const confirmed = window.confirm(
            "Are you sure you want to remove this child category?"
        );

        if (!confirmed) return;

        try {
            if (child.categoryId && !child.isNew) {
                await deleteCategory(child.categoryId);
            }

            setChildForms(prev =>
                prev.filter((_, i) => i !== index)
            );
        } catch (err) {
            console.error(err);

            setError(
                err?.response?.data?.message ||
                "Remove child category failed."
            );
        }
    }

    async function handleSubmit(e) {
        e.preventDefault();

        try {
            setIsSubmitting(true);
            setError("");

            await updateCategory(categoryId, {
                ...parentForm,
                parentCategoryId: parentForm.parentCategoryId || null,
                sortOrder: Number(parentForm.sortOrder || 0),
                isActive: Boolean(parentForm.isActive)
            });

            const validChildren = childForms.filter(child =>
                child.categoryName?.trim() &&
                child.categoryCode?.trim()
            );

            for (const child of validChildren) {
                const childRequest = {
                    parentCategoryId: Number(categoryId),
                    categoryName: child.categoryName,
                    categoryCode: child.categoryCode,
                    description: child.description || "",
                    sortOrder: Number(child.sortOrder || 0),
                    isActive: Boolean(child.isActive)
                };

                if (child.isNew || !child.categoryId) {
                    await createCategory(childRequest);
                } else {
                    await updateCategory(child.categoryId, childRequest);
                }
            }

            navigate("/categories");
        } catch (err) {
            console.error(err);
            setError(
                err?.response?.data?.message ||
                "Update category failed. Please try again."
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    if (isLoading) {
        return (
            <div className="container-fluid px-4 py-4">
                <div className="card border-0 shadow-sm">
                    <div className="card-body p-4 text-muted">
                        Loading category...
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="container-fluid px-4 py-4">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">Edit Category</h3>
                    <p className="text-muted mb-0">
                        Update parent category and manage its child categories.
                    </p>
                </div>

                <button
                    type="button"
                    className="btn btn-outline-secondary"
                    onClick={() => navigate("/categories")}
                >
                    Back
                </button>
            </div>

            {error && (
                <div className="alert alert-danger">
                    {error}
                </div>
            )}

            <form onSubmit={handleSubmit}>
                <div className="card border-0 shadow-sm">
                    <div className="card-body p-4">
                        <div className="d-flex justify-content-between align-items-start mb-4">
                            <div>
                                <h5 className="fw-semibold mb-1">
                                    Category information
                                </h5>

                                <p className="text-muted mb-0">
                                    Edit the main category, then update or add child categories below.
                                </p>
                            </div>

                            <button
                                type="button"
                                className="btn btn-outline-dark btn-sm"
                                onClick={handleAddChild}
                            >
                                + Add Child
                            </button>
                        </div>

                        <div className="border rounded-3 p-3 mb-4 bg-light">
                            <div className="d-flex justify-content-between mb-3">
                                <h6 className="fw-semibold mb-0">
                                    Parent Category
                                </h6>
                            </div>

                            <div className="row g-3">
                                <div className="col-md-6">
                                    <label className="form-label">Category Name</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={parentForm.categoryName}
                                        onChange={(e) =>
                                            handleParentChange("categoryName", e.target.value)
                                        }
                                        required
                                    />
                                </div>

                                <div className="col-md-6">
                                    <label className="form-label">Category Code</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={parentForm.categoryCode}
                                        onChange={(e) =>
                                            handleParentChange("categoryCode", e.target.value)
                                        }
                                        required
                                    />
                                </div>

                                <div className="col-md-8">
                                    <label className="form-label">Description</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={parentForm.description || ""}
                                        onChange={(e) =>
                                            handleParentChange("description", e.target.value)
                                        }
                                        placeholder="Optional"
                                    />
                                </div>

                                <div className="col-md-2">
                                    <label className="form-label">Sort Order</label>
                                    <input
                                        type="number"
                                        className="form-control"
                                        value={parentForm.sortOrder}
                                        onChange={(e) =>
                                            handleParentChange("sortOrder", e.target.value)
                                        }
                                        min="0"
                                    />
                                </div>

                                <div className="col-md-2">
                                    <label className="form-label">Status</label>
                                    <select
                                        className="form-select"
                                        value={parentForm.isActive ? "true" : "false"}
                                        onChange={(e) =>
                                            handleParentChange("isActive", e.target.value === "true")
                                        }
                                    >
                                        <option value="true">Active</option>
                                        <option value="false">Inactive</option>
                                    </select>
                                </div>
                            </div>
                        </div>

                        {childForms.length > 0 && (
                            <div className="mb-3">
                                <h6 className="fw-semibold mb-1">
                                    Child Categories
                                </h6>
                                <p className="text-muted small mb-3">
                                    Existing child categories will be updated. New child categories will be created.
                                </p>
                            </div>
                        )}

                        {childForms.map((form, index) => (
                            <div
                                key={form.categoryId || index}
                                className="border rounded-3 p-3 mb-3"
                            >
                                <div className="d-flex justify-content-between mb-3">
                                    <h6 className="fw-semibold mb-0">
                                        {form.isNew
                                            ? `New Child Category #${index + 1}`
                                            : `Child Category #${index + 1}`}
                                    </h6>

                                    <button
                                        type="button"
                                        className="btn btn-outline-danger btn-sm"
                                        onClick={() => handleRemoveChild(index)}
                                    >
                                        Remove
                                    </button>
                                </div>

                                <div className="row g-3">
                                    <div className="col-md-6">
                                        <label className="form-label">Category Name</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            value={form.categoryName}
                                            onChange={(e) =>
                                                handleChildChange(index, "categoryName", e.target.value)
                                            }
                                        />
                                    </div>

                                    <div className="col-md-6">
                                        <label className="form-label">Category Code</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            value={form.categoryCode}
                                            onChange={(e) =>
                                                handleChildChange(index, "categoryCode", e.target.value)
                                            }
                                        />
                                    </div>

                                    <div className="col-md-8">
                                        <label className="form-label">Description</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            value={form.description || ""}
                                            onChange={(e) =>
                                                handleChildChange(index, "description", e.target.value)
                                            }
                                            placeholder="Optional"
                                        />
                                    </div>

                                    <div className="col-md-2">
                                        <label className="form-label">Sort Order</label>
                                        <input
                                            type="number"
                                            className="form-control"
                                            value={form.sortOrder}
                                            onChange={(e) =>
                                                handleChildChange(index, "sortOrder", e.target.value)
                                            }
                                            min="0"
                                        />
                                    </div>

                                    <div className="col-md-2">
                                        <label className="form-label">Status</label>
                                        <select
                                            className="form-select"
                                            value={form.isActive ? "true" : "false"}
                                            onChange={(e) =>
                                                handleChildChange(index, "isActive", e.target.value === "true")
                                            }
                                        >
                                            <option value="true">Active</option>
                                            <option value="false">Inactive</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                        ))}

                        {childForms.length === 0 && (
                            <div className="border rounded-3 p-4 text-center text-muted mb-3">
                                No child category yet. Click{" "}
                                <span className="fw-semibold">+ Add Child</span>{" "}
                                to create one.
                            </div>
                        )}

                        <div className="d-flex justify-content-end gap-2 mt-4">
                            <button
                                type="button"
                                className="btn btn-outline-secondary"
                                onClick={() => navigate("/categories")}
                                disabled={isSubmitting}
                            >
                                Cancel
                            </button>

                            <button
                                type="submit"
                                className="btn btn-dark"
                                disabled={isSubmitting}
                            >
                                {isSubmitting ? "Saving..." : "Update Category"}
                            </button>
                        </div>
                    </div>
                </div>
            </form>
        </div>
    );
}

export default CategoryEditPage;