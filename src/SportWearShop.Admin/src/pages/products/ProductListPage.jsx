import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";

import { getProducts, deleteProduct } from "../../api/productApi";
import PageHeader from "../../components/common/PageHeader";
import SearchBox from "../../components/common/SearchBox";
import { showToast } from "../../redux/toast/toastSlice";

import {
    productQueryRequestModel,
    productStatusOptions,
    productSortByOptions
} from "../../models/productModel";

function ProductListPage() {
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const [products, setProducts] = useState([]);
    const [query, setQuery] = useState(productQueryRequestModel);
    const [isLoading, setIsLoading] = useState(false);

    const [paging, setPaging] = useState({
        totalCount: 0,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false
    });

    useEffect(() => {
        loadProducts(query);
    }, [
        query.pageNumber,
        query.pageSize,
        query.status,
        query.sortBy,
        query.isAscending
    ]);

    async function loadProducts(currentQuery = query) {
        try {
            setIsLoading(true);

            const data = await getProducts(currentQuery);

            setProducts(data.items || []);

            setPaging({
                totalCount: data.totalCount || 0,
                pageNumber: data.pageNumber || 1,
                pageSize: data.pageSize || currentQuery.pageSize,
                totalPages: data.totalPages || 1,
                hasPreviousPage: data.hasPreviousPage || false,
                hasNextPage: data.hasNextPage || false
            });
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to load products."
                })
            );
        } finally {
            setIsLoading(false);
        }
    }

    function updateQuery(name, value) {
        setQuery((prev) => ({
            ...prev,
            [name]: value,
            pageNumber: 1
        }));
    }

    function handleSearchSubmit(e) {
        e.preventDefault();

        const newQuery = {
            ...query,
            pageNumber: 1
        };

        setQuery(newQuery);
        loadProducts(newQuery);
    }

    function handleResetFilters() {
        const defaultQuery = {
            ...productQueryRequestModel
        };

        setQuery(defaultQuery);
        loadProducts(defaultQuery);
    }

    async function handleDeleteProduct(productId) {
        const confirmed = window.confirm(
            "Are you sure you want to delete this product?"
        );

        if (!confirmed) return;

        try {
            await deleteProduct(productId);

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Product deleted successfully."
                })
            );

            loadProducts(query);
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to delete product."
                })
            );
        }
    }

    function formatPrice(value) {
        return new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND"
        }).format(value || 0);
    }

    function getStatusBadgeClass(status) {
        switch (status) {
            case "Active":
                return "badge bg-success";

            case "Draft":
                return "badge bg-warning text-dark";

            case "Deleted":
                return "badge bg-danger";

            default:
                return "badge bg-secondary";
        }
    }

    return (
        <div>
            <PageHeader
                title="Products"
                description="Manage product information, variants, images and status."
                actionText="+ Add Product"
                onActionClick={() => navigate("/products/create")}
            />

            <div className="card border-0 shadow-sm mb-3">
                <div className="card-body">
                    <form onSubmit={handleSearchSubmit}>
                        <div className="row g-3 align-items-end">
                            <div className="col-md-4">
                                <label className="form-label small text-muted">
                                    Search
                                </label>

                                <SearchBox
                                    value={query.searchTerm}
                                    onChange={(value) =>
                                        setQuery((prev) => ({
                                            ...prev,
                                            searchTerm: value
                                        }))
                                    }
                                    placeholder="Search name, code, brand..."
                                />
                            </div>

                            <div className="col-md-2">
                                <label className="form-label small text-muted">
                                    Status
                                </label>

                                <select
                                    className="form-select"
                                    value={query.status}
                                    onChange={(e) =>
                                        updateQuery("status", e.target.value)
                                    }
                                >
                                    {productStatusOptions.map((option) => (
                                        <option
                                            key={option.label}
                                            value={option.value}
                                        >
                                            {option.label}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="col-md-2">
                                <label className="form-label small text-muted">
                                    Sort by
                                </label>

                                <select
                                    className="form-select"
                                    value={query.sortBy}
                                    onChange={(e) =>
                                        updateQuery("sortBy", Number(e.target.value))
                                    }
                                >
                                    {productSortByOptions.map((option) => (
                                        <option
                                            key={option.value}
                                            value={option.value}
                                        >
                                            {option.label}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="col-md-2">
                                <label className="form-label small text-muted">
                                    Direction
                                </label>

                                <select
                                    className="form-select"
                                    value={query.isAscending ? "true" : "false"}
                                    onChange={(e) =>
                                        updateQuery(
                                            "isAscending",
                                            e.target.value === "true"
                                        )
                                    }
                                >
                                    <option value="false">Descending</option>
                                    <option value="true">Ascending</option>
                                </select>
                            </div>

                            <div className="col-md-2 d-flex gap-2">
                                <button type="submit" className="btn btn-dark">
                                    Search
                                </button>

                                <button
                                    type="button"
                                    className="btn btn-outline-secondary"
                                    onClick={handleResetFilters}
                                >
                                    Reset
                                </button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>

            <div className="card border-0 shadow-sm">
                <div className="card-body">
                    <div className="d-flex justify-content-between align-items-center mb-3">
                        <h5 className="mb-0 fw-semibold">Product List</h5>

                        <div className="text-muted small">
                            Total: {paging.totalCount} products
                        </div>
                    </div>

                    {isLoading ? (
                        <div className="text-center py-5 text-muted">
                            Loading products...
                        </div>
                    ) : (
                        <>
                            <div className="table-responsive">
                                <table className="table table-hover align-middle">
                                    <thead className="table-light">
                                        <tr>
                                            <th>#</th>
                                            <th>Image</th>
                                            <th>Product</th>
                                            <th>Brand</th>
                                            <th>Category</th>
                                            <th>Gender</th>
                                            <th>Price</th>
                                            <th>Variants</th>
                                            <th>Status</th>
                                            <th className="text-end">
                                                Actions
                                            </th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        {products.length === 0 ? (
                                            <tr>
                                                <td
                                                    colSpan="10"
                                                    className="text-center text-muted py-4"
                                                >
                                                    No products found.
                                                </td>
                                            </tr>
                                        ) : (
                                            products.map((product, index) => (
                                                <tr key={product.productId}>
                                                    <td>
                                                        {(paging.pageNumber - 1) *
                                                            paging.pageSize +
                                                            index +
                                                            1}
                                                    </td>

                                                    <td>
                                                        {product.thumbnailUrl ? (
                                                            <img
                                                                src={product.thumbnailUrl}
                                                                alt={product.productName}
                                                                width="56"
                                                                height="56"
                                                                className="rounded object-fit-cover"
                                                            />
                                                        ) : (
                                                            <div
                                                                className="bg-light rounded d-flex align-items-center justify-content-center"
                                                                style={{
                                                                    width: "56px",
                                                                    height: "56px"
                                                                }}
                                                            >
                                                                <i className="bi bi-image text-muted"></i>
                                                            </div>
                                                        )}
                                                    </td>

                                                    <td>
                                                        <div className="fw-medium">
                                                            {product.productName}
                                                        </div>

                                                        <div className="text-muted small">
                                                            {product.productCode}
                                                        </div>
                                                    </td>

                                                    <td>{product.brandName}</td>

                                                    <td>{product.categoryName}</td>

                                                    <td>{product.gender}</td>

                                                    <td>
                                                        {product.minSalePrice ? (
                                                            <>
                                                                <div className="fw-medium text-danger">
                                                                    {formatPrice(product.minSalePrice)}
                                                                </div>

                                                                <div className="text-muted small text-decoration-line-through">
                                                                    {formatPrice(product.minPrice)}
                                                                </div>
                                                            </>
                                                        ) : (
                                                            <span className="fw-medium">
                                                                {formatPrice(product.minPrice)}
                                                            </span>
                                                        )}
                                                    </td>

                                                    <td>
                                                        <span className="badge bg-light text-dark">
                                                            {product.totalVariants}
                                                        </span>
                                                    </td>

                                                    <td>
                                                        <span className={getStatusBadgeClass(product.status)}>
                                                            {product.status}
                                                        </span>
                                                    </td>

                                                    <td className="text-end">
                                                        <Link
                                                            to={`/products/${product.productId}`}
                                                            className="btn btn-sm btn-outline-primary me-2"
                                                        >
                                                            View
                                                        </Link>

                                                        <Link
                                                            to={`/products/${product.productId}/update`}
                                                            className="btn btn-sm btn-outline-dark me-2"
                                                        >
                                                            Update
                                                        </Link>

                                                        <button
                                                            type="button"
                                                            className="btn btn-sm btn-outline-danger"
                                                            onClick={() =>
                                                                handleDeleteProduct(product.productId)
                                                            }
                                                        >
                                                            Delete
                                                        </button>
                                                    </td>
                                                </tr>
                                            ))
                                        )}
                                    </tbody>
                                </table>
                            </div>

                            <div className="d-flex justify-content-between align-items-center mt-3">
                                <div className="text-muted small">
                                    Page {paging.pageNumber} of {paging.totalPages}
                                </div>

                                <div>
                                    <button
                                        type="button"
                                        className="btn btn-sm btn-outline-secondary me-2"
                                        disabled={!paging.hasPreviousPage}
                                        onClick={() =>
                                            setQuery((prev) => ({
                                                ...prev,
                                                pageNumber: prev.pageNumber - 1
                                            }))
                                        }
                                    >
                                        Previous
                                    </button>

                                    <button
                                        type="button"
                                        className="btn btn-sm btn-outline-secondary"
                                        disabled={!paging.hasNextPage}
                                        onClick={() =>
                                            setQuery((prev) => ({
                                                ...prev,
                                                pageNumber: prev.pageNumber + 1
                                            }))
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

export default ProductListPage;