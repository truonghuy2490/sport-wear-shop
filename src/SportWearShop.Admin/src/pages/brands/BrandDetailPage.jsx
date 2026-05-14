import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";

import { getBrandById } from "../../api/brandApi";
import StatusBadge from "../../components/common/StatusBadge";

function BrandDetailPage() {
    const { brandId } = useParams();

    const [brand, setBrand] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        loadBrandDetail();
    }, [brandId]);

    async function loadBrandDetail() {
        try {
            setIsLoading(true);

            const data = await getBrandById(brandId);

            setBrand(data);
        } catch (error) {
            console.error("Failed to load brand detail:", error);
        } finally {
            setIsLoading(false);
        }
    }

    if (isLoading) {
        return (
            <div className="text-center py-5 text-muted">
                Loading brand detail...
            </div>
        );
    }

    if (!brand) {
        return (
            <div className="text-center py-5 text-muted">
                Brand not found.
            </div>
        );
    }

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">
                        Brand Detail
                    </h3>

                    <p className="text-muted mb-0">
                        View brand information and product association.
                    </p>
                </div>

                <div>
                    <Link
                        to="/brands"
                        className="btn btn-outline-secondary me-2"
                    >
                        Back
                    </Link>

                    <Link
                        to={`/brands/${brand.brandId}/edit`}
                        className="btn btn-dark"
                    >
                        Edit Brand
                    </Link>
                </div>
            </div>

            <div className="card border-0 shadow-sm">
                <div className="card-body p-4">

                    <div className="d-flex align-items-center gap-4 mb-4">
                        {brand.brandImage ? (
                            <img
                                src={brand.brandImage}
                                alt={brand.brandName}
                                width="96"
                                height="96"
                                className="rounded object-fit-cover"
                            />
                        ) : (
                            <div
                                className="bg-light rounded d-flex align-items-center justify-content-center"
                                style={{
                                    width: "96px",
                                    height: "96px"
                                }}
                            >
                                <i className="bi bi-image fs-2 text-muted"></i>
                            </div>
                        )}

                        <div>
                            <h4 className="fw-bold mb-1">
                                {brand.brandName}
                            </h4>

                            <div className="text-muted">
                                {brand.brandCode}
                            </div>

                            <div className="mt-2">
                                <StatusBadge isActive={brand.isActive} />
                            </div>
                        </div>
                    </div>

                    <hr />

                    <div className="row g-4 mt-1">
                        <div className="col-md-6">
                            <div className="text-muted small">
                                Brand ID
                            </div>
                            <div className="fw-medium">
                                {brand.brandId}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Product Count
                            </div>
                            <div className="fw-medium">
                                {brand.productCount}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Created At
                            </div>
                            <div className="fw-medium">
                                {new Date(brand.createdAtUtc).toLocaleString()}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Updated At
                            </div>
                            <div className="fw-medium">
                                {new Date(brand.updatedAtUtc).toLocaleString()}
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    );
}

export default BrandDetailPage;