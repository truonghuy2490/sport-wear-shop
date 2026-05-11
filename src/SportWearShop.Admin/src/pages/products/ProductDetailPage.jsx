import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useDispatch } from "react-redux";

import { getProductDetail } from "../../api/productApi";
import { showToast } from "../../redux/toast/toastSlice";

function ProductDetailPage() {
    const { productId } = useParams();

    const dispatch = useDispatch();

    const [product, setProduct] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        loadProductDetail();
    }, [productId]);

    async function loadProductDetail() {
        try {
            setIsLoading(true);

            const data = await getProductDetail(productId);

            setProduct(data);
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to load product detail."
                })
            );
        } finally {
            setIsLoading(false);
        }
    }

    if (isLoading) {
        return (
            <div className="text-center py-5 text-muted">
                Loading product detail...
            </div>
        );
    }

    if (!product) {
        return (
            <div className="text-center py-5 text-muted">
                Product not found.
            </div>
        );
    }

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">
                        Product Detail
                    </h3>

                    <p className="text-muted mb-0">
                        View product information, images and variants.
                    </p>
                </div>

                <div>
                    <Link
                        to="/products"
                        className="btn btn-outline-secondary me-2"
                    >
                        Back
                    </Link>

                    <Link
                        to={`/products/${product.productId}/edit`}
                        className="btn btn-dark"
                    >
                        Edit Product
                    </Link>
                </div>
            </div>

            <div className="card border-0 shadow-sm mb-4">
                <div className="card-body p-4">
                    <div className="d-flex justify-content-between align-items-start mb-3">
                        <div>
                            <h4 className="fw-bold mb-1">
                                {product.productName}
                            </h4>

                            <div className="text-muted">
                                {product.productCode}
                            </div>
                        </div>

                        <span
                            className={
                                product.status === "Active"
                                    ? "badge bg-success"
                                    : "badge bg-secondary"
                            }
                        >
                            {product.status}
                        </span>
                    </div>

                    <hr />

                    <div className="row g-4">
                        <div className="col-md-6">
                            <div className="text-muted small">Product ID</div>
                            <div className="fw-medium">{product.productId}</div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">Slug</div>
                            <div className="fw-medium">{product.slug}</div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">Brand</div>
                            <div className="fw-medium">{product.brandName}</div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">Category</div>
                            <div className="fw-medium">{product.categoryName}</div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">Gender</div>
                            <div className="fw-medium">{product.gender}</div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">Base Material</div>
                            <div className="fw-medium">
                                {product.baseMaterial || "-"}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">Created At</div>
                            <div className="fw-medium">
                                {new Date(product.createdAtUtc).toLocaleString()}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">Updated At</div>
                            <div className="fw-medium">
                                {new Date(product.updatedAtUtc).toLocaleString()}
                            </div>
                        </div>

                        <div className="col-md-12">
                            <div className="text-muted small">Description</div>
                            <div className="fw-medium">
                                {product.description || "-"}
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div className="card border-0 shadow-sm mb-4">
                <div className="card-body">
                    <h5 className="fw-semibold mb-3">
                        Product Images
                    </h5>

                    {product.images?.length === 0 ? (
                        <div className="text-muted">
                            No images found.
                        </div>
                    ) : (
                        <div className="row g-3">
                            {product.images.map((image) => (
                                <div
                                    className="col-6 col-md-3 col-lg-2"
                                    key={image.productImageId}
                                >
                                    <div className="border rounded-3 p-2 h-100">
                                        <img
                                            src={image.imageUrl}
                                            alt={image.altText || product.productName}
                                            className="img-fluid rounded mb-2"
                                            style={{
                                                height: "120px",
                                                width: "100%",
                                                objectFit: "cover"
                                            }}
                                        />

                                        {image.isMain && (
                                            <span className="badge bg-dark">
                                                Main
                                            </span>
                                        )}
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>

            <div className="card border-0 shadow-sm">
                <div className="card-body">
                    <h5 className="fw-semibold mb-3">
                        Product Variants
                    </h5>

                    <div className="table-responsive">
                        <table className="table table-hover align-middle">
                            <thead className="table-light">
                                <tr>
                                    <th>#</th>
                                    <th>SKU</th>
                                    <th>Color</th>
                                    <th>Size</th>
                                    <th>List Price</th>
                                    <th>Sale Price</th>
                                    <th>Weight</th>
                                    <th>Status</th>
                                </tr>
                            </thead>

                            <tbody>
                                {product.variants?.length === 0 ? (
                                    <tr>
                                        <td
                                            colSpan="8"
                                            className="text-center text-muted py-4"
                                        >
                                            No variants found.
                                        </td>
                                    </tr>
                                ) : (
                                    product.variants.map((variant, index) => (
                                        <tr key={variant.productVariantId}>
                                            <td>{index + 1}</td>

                                            <td className="fw-medium">
                                                {variant.sku}
                                            </td>

                                            <td>
                                                {variant.colorName || "-"}
                                            </td>

                                            <td>
                                                {variant.sizeLabel || "-"}
                                            </td>

                                            <td>
                                                ${Number(variant.listPrice).toFixed(2)}
                                            </td>

                                            <td>
                                                {variant.salePrice
                                                    ? `$${Number(variant.salePrice).toFixed(2)}`
                                                    : "-"}
                                            </td>

                                            <td>
                                                {variant.weightGrams
                                                    ? `${variant.weightGrams}g`
                                                    : "-"}
                                            </td>

                                            <td>
                                                <span
                                                    className={
                                                        variant.status === "Active"
                                                            ? "badge bg-success"
                                                            : "badge bg-secondary"
                                                    }
                                                >
                                                    {variant.status}
                                                </span>
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ProductDetailPage;