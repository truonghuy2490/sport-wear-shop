import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useDispatch } from "react-redux";

import { getBrandById, updateBrand } from "../../api/brandApi";
import { showToast } from "../../redux/toast/toastSlice";

function BrandEditPage() {
    const { brandId } = useParams();

    const navigate = useNavigate();
    const dispatch = useDispatch();

    const [formData, setFormData] = useState({
        brandName: "",
        brandCode: "",
        brandImage: "",
        isActive: true
    });

    const [isLoading, setIsLoading] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        loadBrandDetail();
    }, [brandId]);

    async function loadBrandDetail() {
        try {
            setIsLoading(true);

            const data = await getBrandById(brandId);

            setFormData({
                brandName: data.brandName || "",
                brandCode: data.brandCode || "",
                brandImage: data.brandImage || "",
                isActive: data.isActive
            });
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to load brand detail."
                })
            );
        } finally {
            setIsLoading(false);
        }
    }

    function handleChange(e) {
        const { name, value, type, checked } = e.target;

        setFormData((prev) => ({
            ...prev,
            [name]: type === "checkbox" ? checked : value
        }));
    }

    async function handleSubmit(e) {
        e.preventDefault();

        try {
            setIsSubmitting(true);

            await updateBrand(brandId, formData);

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Brand updated successfully."
                })
            );

            navigate(`/brands/${brandId}`);
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to update brand."
                })
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    if (isLoading) {
        return (
            <div className="text-center py-5 text-muted">
                Loading brand form...
            </div>
        );
    }

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">
                        Edit Brand
                    </h3>

                    <p className="text-muted mb-0">
                        Update brand information and status.
                    </p>
                </div>

                <Link
                    to={`/brands/${brandId}`}
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
                                    Brand Image URL
                                </label>

                                <input
                                    type="text"
                                    name="brandImage"
                                    className="form-control"
                                    value={formData.brandImage}
                                    onChange={handleChange}
                                    placeholder="https://..."
                                />
                            </div>

                            <div className="col-md-12">
                                <div className="form-check">
                                    <input
                                        type="checkbox"
                                        name="isActive"
                                        className="form-check-input"
                                        id="isActive"
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
                                {isSubmitting ? "Saving..." : "Save Changes"}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}

export default BrandEditPage;