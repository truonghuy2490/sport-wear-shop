import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createCategory } from "../../api/categoryApi";
import { createCategoryRequestModel } from "../../models/categoryModel";

function CategoryCreatePage() {
    const navigate = useNavigate();

    const [parentForm, setParentForm] = useState({
        ...createCategoryRequestModel,
        parentCategoryId: null
    });

    const [childForms, setChildForms] = useState([]);

    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState("");

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
                parentCategoryId: null,
                sortOrder: prev.length + 1
            }
        ]);
    }

    function handleRemoveChild(index) {
        setChildForms(prev => prev.filter((_, i) => i !== index));
    }

    async function handleSubmit(e) {
        e.preventDefault();

        try {
            setIsSubmitting(true);
            setError("");

            const parentRequest = {
                ...parentForm,
                parentCategoryId: null,
                sortOrder: Number(parentForm.sortOrder || 0)
            };

            const createdParent = await createCategory(parentRequest);

            const validChildren = childForms.filter(child =>
                child.categoryName?.trim() &&
                child.categoryCode?.trim()
            );

            for (const child of validChildren) {
                await createCategory({
                    ...child,
                    parentCategoryId: createdParent.categoryId,
                    sortOrder: Number(child.sortOrder || 0)
                });
            }

            navigate("/categories");
        } catch (err) {
            console.error(err);
            setError(
                err?.response?.data?.message ||
                "Create category failed. Please try again."
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    return (
        <div className="container-fluid px-4 py-4">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">Create Category</h3>
                    <p className="text-muted mb-0">
                        Create a parent category and add child categories in one flow.
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
                                    Start with the main category, then add its child categories below.
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

                                <div className="col-md-9">
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

                                <div className="col-md-3">
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
                            </div>
                        </div>

                        {childForms.length > 0 && (
                            <div className="mb-3">
                                <h6 className="fw-semibold mb-1">
                                    Child Categories
                                </h6>
                                <p className="text-muted small mb-3">
                                    These categories will be created under the parent category above.
                                </p>
                            </div>
                        )}

                        {childForms.map((form, index) => (
                            <div
                                key={index}
                                className="border rounded-3 p-3 mb-3"
                            >
                                <div className="d-flex justify-content-between mb-3">
                                    <h6 className="fw-semibold mb-0">
                                        Child Category #{index + 1}
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

                                    <div className="col-md-9">
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

                                    <div className="col-md-3">
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
                                {isSubmitting ? "Saving..." : "Create Category"}
                            </button>
                        </div>
                    </div>
                </div>
            </form>
        </div>
    );
}

export default CategoryCreatePage;