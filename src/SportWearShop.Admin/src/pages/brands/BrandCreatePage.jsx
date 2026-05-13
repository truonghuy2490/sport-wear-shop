import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";

import { createBrand } from "../../api/brandApi";
import { createBrandRequestModel } from "../../models/brandModel";
import { showToast } from "../../redux/toast/toastSlice";

function BrandCreatePage() {
    const navigate = useNavigate();
    const dispatch = useDispatch();

    const [formData, setFormData] = useState({
        ...createBrandRequestModel
    });

    const [previewImage, setPreviewImage] = useState("");
    const [selectedFileName, setSelectedFileName] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);

    function handleChange(e) {
        const { name, value } = e.target;

        setFormData((prev) => ({
            ...prev,
            [name]: value
        }));
    }

    function handleImageChange(e) {
        const file = e.target.files?.[0];

        if (!file) {
            setFormData((prev) => ({
                ...prev,
                brandImageFile: null
            }));

            setPreviewImage("");
            setSelectedFileName("");
            return;
        }

        setFormData((prev) => ({
            ...prev,
            brandImageFile: file
        }));

        setPreviewImage(URL.createObjectURL(file));
        setSelectedFileName(file.name);
    }

    async function handleSubmit(e) {
        e.preventDefault();

        try {
            setIsSubmitting(true);

            const result = await createBrand(formData);

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Brand created successfully."
                })
            );

            navigate(`/brands/${result.brandId}`);
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to create brand."
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
                    <h3 className="fw-bold mb-1">Create Brand</h3>
                    <p className="text-muted mb-0">
                        Add a new brand to the system.
                    </p>
                </div>

                <Link to="/brands" className="btn btn-outline-secondary">
                    Back
                </Link>
            </div>

            <div className="card border-0 shadow-sm">
                <div className="card-body p-4">
                    <form onSubmit={handleSubmit}>
                        <div className="row g-3">
                            <div className="col-md-6">
                                <label className="form-label fw-medium">
                                    Brand Name
                                </label>

                                <input
                                    type="text"
                                    name="brandName"
                                    className="form-control"
                                    value={formData.brandName}
                                    onChange={handleChange}
                                    required
                                />
                            </div>

                            <div className="col-md-6">
                                <label className="form-label fw-medium">
                                    Brand Code
                                </label>

                                <input
                                    type="text"
                                    name="brandCode"
                                    className="form-control"
                                    value={formData.brandCode}
                                    onChange={handleChange}
                                    required
                                />
                            </div>

                            <div className="col-md-12">
                                <label className="form-label fw-medium">
                                    Brand Image
                                </label>

                                <input
                                    type="file"
                                    name="brandImageFile"
                                    className="form-control"
                                    accept="image/*"
                                    onChange={handleImageChange}
                                />

                                <small className="text-muted">
                                    Upload brand logo or image.
                                </small>

                                {selectedFileName && (
                                    <div className="small text-muted mt-1">
                                        Selected file: {selectedFileName}
                                    </div>
                                )}

                                {previewImage ? (
                                    <div className="mt-3">
                                        <img
                                            src={previewImage}
                                            alt="Brand preview"
                                            width="120"
                                            height="120"
                                            className="rounded border object-fit-cover"
                                        />
                                    </div>
                                ) : (
                                    <div
                                        className="mt-3 bg-light rounded border d-flex align-items-center justify-content-center"
                                        style={{
                                            width: "120px",
                                            height: "120px"
                                        }}
                                    >
                                        <i className="bi bi-image text-muted fs-3"></i>
                                    </div>
                                )}
                            </div>
                        </div>

                        <div className="d-flex justify-content-end gap-2 mt-4">
                            <Link
                                to="/brands"
                                className="btn btn-outline-secondary"
                            >
                                Cancel
                            </Link>

                            <button
                                type="submit"
                                className="btn btn-dark"
                                disabled={isSubmitting}
                            >
                                {isSubmitting ? "Creating..." : "Create Brand"}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}

export default BrandCreatePage;