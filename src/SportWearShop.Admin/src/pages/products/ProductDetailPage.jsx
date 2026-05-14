import { useEffect, useState } from "react";
import { Link, useParams} from "react-router-dom";
import { useDispatch } from "react-redux";
import { productDetailResponseModel } from "../../models/productModel";
import { getProductDetail, updateProductStatus } from "../../api/productApi";
import { updateProductVariantStatus } from "../../api/productVariantApi";
import { showToast } from "../../redux/toast/toastSlice";

function ProductDetailPage() {
    
    const { productId } = useParams();
    const dispatch = useDispatch();

    const [product, setProduct] = useState(productDetailResponseModel);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        loadProductDetail();
    }, [productId]);

    async function loadProductDetail() {
        try {
            setIsLoading(true);

            const data = await getProductDetail(productId);

            setProduct({
                ...productDetailResponseModel,
                ...data,
                images: data.images || [],
                variants: data.variants || []
            });
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

    function formatPrice(value) {
        return new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND"
        }).format(value || 0);
    }

    function formatDate(value) {
        if (!value) return "-";
        return new Date(value).toLocaleString("vi-VN");
    }

    function getStatusBadgeClass(status) {
        switch (status) {
            case "Active":
                return "badge bg-success";
            case "Draft":
                return "badge bg-warning text-dark";
            case "Deleted":
                return "badge bg-danger";
            case "Inactive":
                return "badge bg-secondary";
            default:
                return "badge bg-secondary";
        }
    }

    function getStockBadgeClass(availableQuantity) {
        if (availableQuantity <= 0) return "badge bg-danger";
        if (availableQuantity <= 5) return "badge bg-warning text-dark";
        return "badge bg-success";
    }

    async function handleUpdateProductStatus(status) {
        try {
            await updateProductStatus(product.productId, status);
            await loadProductDetail();

            dispatch(showToast({
                type: "success",
                title: "Success",
                message: "Product status updated."
            }));
        } catch (error) {
            dispatch(showToast({
                type: "error",
                title: "Error",
                message: error.response?.data?.message || "Failed to update product status."
            }));
        }
    }

    async function handleUpdateVariantStatus(productVariantId, status) {
        try {
            await updateProductVariantStatus(productVariantId, status);
            await loadProductDetail();

            dispatch(showToast({
                type: "success",
                title: "Success",
                message: "Variant status updated."
            }));
        } catch (error) {
            dispatch(showToast({
                type: "error",
                title: "Error",
                message: error.response?.data?.message || "Failed to update variant status."
            }));
        }
    }

    if (isLoading) {
        return (
            <div className="text-center py-5 text-muted">
                Loading product detail...
            </div>
        );
    }

    if (!product.productId) {
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
                    <h3 className="fw-bold mb-1">Admin Product Detail</h3>

                    <p className="text-muted mb-0">
                        View full admin product information, images, variants and inventory.
                    </p>
                </div>

                <div>
                    <Link
                        to="/products"
                        className="btn btn-outline-secondary me-2"
                    >
                        Back
                    </Link>

                    {product.status !== "Deleted" && (
                        <Link
                            to={`/products/${product.productId}/update`}
                            className="btn btn-dark"
                        >
                            Update Product
                        </Link>
                    )}
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

                        <div className="d-flex align-items-center gap-2">
                            {product.status !== "Deleted" && product.status === "Draft" && (
                                <button
                                    type="button"
                                    className="btn btn-sm btn-success"
                                    onClick={() => handleUpdateProductStatus(1)}
                                >
                                    Publish
                                </button>
                            )}

                            {product.status !== "Deleted" && product.status === "Active" && (
                                <button
                                    type="button"
                                    className="btn btn-sm btn-warning"
                                    onClick={() => handleUpdateProductStatus(0)}
                                >
                                    Unpublish
                                </button>
                            )}

                            <span className={getStatusBadgeClass(product.status)}>
                                {product.status}
                            </span>
                        </div>
                    </div>

                    <hr />

                    <div className="row g-4">
                        <ProductInfoItem
                            label="Product ID"
                            value={product.productId}
                        />

                        <ProductInfoItem
                            label="Slug"
                            value={product.slug}
                        />

                        <ProductInfoItem
                            label="Brand"
                            value={`${product.brandName} (#${product.brandId})`}
                        />

                        <ProductInfoItem
                            label="Category"
                            value={`${product.categoryName} (#${product.categoryId})`}
                        />

                        <ProductInfoItem
                            label="Gender"
                            value={product.gender}
                        />

                        <ProductInfoItem
                            label="Base Material"
                            value={product.baseMaterial || "-"}
                        />

                        <ProductInfoItem
                            label="Created At"
                            value={formatDate(product.createdAtUtc)}
                        />

                        <ProductInfoItem
                            label="Updated At"
                            value={formatDate(product.updatedAtUtc)}
                        />

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
                    <div className="d-flex justify-content-between align-items-center mb-3">
                        <h5 className="fw-semibold mb-0">
                            Product Images
                        </h5>

                        <span className="text-muted small">
                            {product.images.length} images
                        </span>
                    </div>

                    {product.images.length === 0 ? (
                        <div className="text-muted">
                            No images found.
                        </div>
                    ) : (
                        <div className="row g-3">
                            {product.images
                                .slice()
                                .sort((a, b) => a.sortOrder - b.sortOrder)
                                .map((image) => (
                                    <div
                                        className="col-6 col-md-3 col-lg-2"
                                        key={image.productImageId}
                                    >
                                        <div className="border rounded-3 p-2 h-100 position-relative">
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

                                            <div className="d-flex justify-content-between align-items-center">
                                                <span className="text-muted small">
                                                    #{image.sortOrder}
                                                </span>

                                                {image.isPrimary && (
                                                    <span className="badge bg-dark">
                                                        Primary
                                                    </span>
                                                )}
                                            </div>

                                            <div className="text-muted small mt-1">
                                                {image.productVariantId
                                                    ? `Variant ID: ${image.productVariantId}`
                                                    : "Product image"}
                                            </div>
                                        </div>
                                    </div>
                                ))}
                        </div>
                    )}
                </div>
            </div>

            <div className="card border-0 shadow-sm">
                <div className="card-body">
                    <div className="d-flex justify-content-between align-items-center mb-3">
                        <h5 className="fw-semibold mb-0">
                            Product Variants
                        </h5>

                        <span className="text-muted small">
                            {product.variants.length} variants
                        </span>
                    </div>

                    <div className="table-responsive">
                        <table className="table table-hover align-middle">
                            <thead className="table-light">
                                <tr>
                                    <th>#</th>
                                    <th>SKU</th>
                                    <th>Color</th>
                                    <th>Size</th>
                                    <th>Price</th>
                                    <th>Weight</th>
                                    <th>On Hand</th>
                                    <th>Reserved</th>
                                    <th>Available</th>
                                    <th>Status</th>
                                    <th>Images</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>

                            <tbody>
                                {product.variants.length === 0 ? (
                                    <tr>
                                        <td
                                            colSpan="12"
                                            className="text-center text-muted py-4"
                                        >
                                            No variants found.
                                        </td>
                                    </tr>
                                ) : (
                                    product.variants.map((variant, index) => (
                                        <tr key={variant.productVariantId}>
                                            <td>{index + 1}</td>

                                            <td>
                                                <div className="fw-medium">
                                                    {variant.sku}
                                                </div>

                                                <div className="text-muted small">
                                                    ID: {variant.productVariantId}
                                                </div>
                                            </td>

                                            <td>
                                                <div className="d-flex align-items-center gap-2">
                                                    <span
                                                        className="border rounded-circle"
                                                        style={{
                                                            width: "18px",
                                                            height: "18px",
                                                            backgroundColor:
                                                                variant.colorCode || "#ffffff"
                                                        }}
                                                    />

                                                    <div>
                                                        <div className="fw-medium">
                                                            {variant.colorName || "-"}
                                                        </div>

                                                        <div className="text-muted small">
                                                            {variant.colorCode || "-"}
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>

                                            <td>
                                                <div className="fw-medium">
                                                    {variant.sizeLabel || "-"}
                                                </div>

                                                <div className="text-muted small">
                                                    {variant.sizeCode || "-"}
                                                </div>
                                            </td>

                                            <td>
                                                {variant.salePrice ? (
                                                    <>
                                                        <div className="fw-medium text-danger">
                                                            {formatPrice(variant.salePrice)}
                                                        </div>

                                                        <div className="text-muted small text-decoration-line-through">
                                                            {formatPrice(variant.listPrice)}
                                                        </div>
                                                    </>
                                                ) : (
                                                    <span className="fw-medium">
                                                        {formatPrice(variant.listPrice)}
                                                    </span>
                                                )}
                                            </td>

                                            <td>
                                                {variant.weightGrams
                                                    ? `${variant.weightGrams}g`
                                                    : "-"}
                                            </td>

                                            <td>{variant.quantityOnHand}</td>

                                            <td>{variant.quantityReserved}</td>

                                            <td>
                                                <span
                                                    className={getStockBadgeClass(
                                                        variant.availableQuantity
                                                    )}
                                                >
                                                    {variant.availableQuantity}
                                                </span>
                                            </td>

                                            <td>
                                                <span className={getStatusBadgeClass(variant.status)}>
                                                    {variant.status}
                                                </span>
                                            </td>

                                            <td>
                                                {variant.images?.length > 0 ? (
                                                    <div className="d-flex">
                                                        {variant.images
                                                            .slice(0, 3)
                                                            .map((image) => (
                                                                <img
                                                                    key={image.productImageId}
                                                                    src={image.imageUrl}
                                                                    alt={
                                                                        image.altText ||
                                                                        variant.sku
                                                                    }
                                                                    className="rounded border me-1"
                                                                    style={{
                                                                        width: "36px",
                                                                        height: "36px",
                                                                        objectFit: "cover"
                                                                    }}
                                                                />
                                                            ))}

                                                        {variant.images.length > 3 && (
                                                            <span className="badge bg-light text-dark align-self-center">
                                                                +{variant.images.length - 3}
                                                            </span>
                                                        )}
                                                    </div>
                                                ) : (
                                                    <span className="text-muted small">
                                                        No images
                                                    </span>
                                                )}
                                            </td>
                                            <td>
                                                <div className="d-flex gap-2">
                                                    {variant.status === "Draft" && (
                                                        <button
                                                            type="button"
                                                            className="btn btn-sm btn-success"
                                                            onClick={() =>
                                                                handleUpdateVariantStatus(variant.productVariantId, 1)
                                                            }
                                                        >
                                                            Publish
                                                        </button>
                                                    )}

                                                    {variant.status === "Active" && (
                                                        <button
                                                            type="button"
                                                            className="btn btn-sm btn-warning"
                                                            onClick={() =>
                                                                handleUpdateVariantStatus(variant.productVariantId, 0)
                                                            }
                                                        >
                                                            Unpublish
                                                        </button>
                                                    )}

                                                    <Link
                                                        to={`/inventory/${variant.productVariantId}`}
                                                        className="btn btn-sm btn-outline-dark"
                                                    >
                                                        Inventory
                                                    </Link>
                                                </div>
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

function ProductInfoItem({ label, value }) {
    return (
        <div className="col-md-6">
            <div className="text-muted small">{label}</div>
            <div className="fw-medium">{value || "-"}</div>
        </div>
    );
}

export default ProductDetailPage;