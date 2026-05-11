import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";

import { getProducts, deleteProduct } from "../../api/productApi";

import PageHeader from "../../components/common/PageHeader";
import SearchBox from "../../components/common/SearchBox";
import { showToast } from "../../redux/toast/toastSlice";

function ProductListPage() {
    const dispatch = useDispatch();

    const [products, setProducts] = useState([]);
    const [searchKeyword, setSearchKeyword] = useState("");
    const [isLoading, setIsLoading] = useState(true);

    const [pageNumber, setPageNumber] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(1);

    const navigate = useNavigate();

    useEffect(() => {
        loadProducts();
    }, [pageNumber]);

    async function loadProducts() {
        try {
            setIsLoading(true);

            const data = await getProducts(pageNumber, pageSize);

            setProducts(data.items || data.data || []);
            setTotalPages(data.totalPages || 1);
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

    async function handleDeleteProduct(productId) {
        const confirmed = window.confirm(
            "Are you sure you want to delete this product?"
        );

        if (!confirmed) return;

        try {
            await deleteProduct(productId);

            setProducts((prevProducts) =>
                prevProducts.filter(
                    (product) => product.productId !== productId
                )
            );

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Product deleted successfully."
                })
            );
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

    const filteredProducts = products.filter((product) => {
        const keyword = searchKeyword.toLowerCase();

        return (
            product.productName?.toLowerCase().includes(keyword) ||
            product.productCode?.toLowerCase().includes(keyword) ||
            product.brandName?.toLowerCase().includes(keyword) ||
            product.categoryName?.toLowerCase().includes(keyword)
        );
    });

    return (
        <div>
            <PageHeader
                title="Products"
                description="Manage product information, variants, images and status."
                actionText="+ Add Product"
                onActionClick={() => navigate("/products/create")}
            />

            <div className="card border-0 shadow-sm">
                <div className="card-body">

                    <div className="d-flex justify-content-between align-items-center mb-3">
                        <h5 className="mb-0 fw-semibold">
                            Product List
                        </h5>

                        <SearchBox
                            value={searchKeyword}
                            onChange={setSearchKeyword}
                            placeholder="Search product..."
                        />
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
                                        {filteredProducts.length === 0 ? (
                                            <tr>
                                                <td
                                                    colSpan="10"
                                                    className="text-center text-muted py-4"
                                                >
                                                    No products found.
                                                </td>
                                            </tr>
                                        ) : (
                                            filteredProducts.map((product, index) => (
                                                <tr key={product.productId}>
                                                    <td>
                                                        {(pageNumber - 1) * pageSize + index + 1}
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

                                                    <td>
                                                        {product.brandName}
                                                    </td>

                                                    <td>
                                                        {product.categoryName}
                                                    </td>

                                                    <td>
                                                        {product.gender}
                                                    </td>

                                                    <td>
                                                        {product.minSalePrice ? (
                                                            <>
                                                                <div className="fw-medium">
                                                                    ${product.minSalePrice.toFixed(2)}
                                                                </div>

                                                                <div className="text-muted small text-decoration-line-through">
                                                                    ${product.minPrice.toFixed(2)}
                                                                </div>
                                                            </>
                                                        ) : (
                                                            <span className="fw-medium">
                                                                ${product.minPrice.toFixed(2)}
                                                            </span>
                                                        )}
                                                    </td>

                                                    <td>
                                                        {product.totalVariants}
                                                    </td>

                                                    <td>
                                                        <span
                                                            className={
                                                                product.status === "Active"
                                                                    ? "badge bg-success"
                                                                    : "badge bg-secondary"
                                                            }
                                                        >
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
                                                            to={`/products/${product.productId}/edit`}
                                                            className="btn btn-sm btn-outline-dark me-2"
                                                        >
                                                            Edit
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
                                    Page {pageNumber} of {totalPages}
                                </div>

                                <div>
                                    <button
                                        type="button"
                                        className="btn btn-sm btn-outline-secondary me-2"
                                        disabled={pageNumber <= 1}
                                        onClick={() => setPageNumber((prev) => prev - 1)}
                                    >
                                        Previous
                                    </button>

                                    <button
                                        type="button"
                                        className="btn btn-sm btn-outline-secondary"
                                        disabled={pageNumber >= totalPages}
                                        onClick={() => setPageNumber((prev) => prev + 1)}
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