import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";

import { getBrands, deleteBrand } from "../../api/brandApi";

import PageHeader from "../../components/common/PageHeader";
import SearchBox from "../../components/common/SearchBox";
import StatusBadge from "../../components/common/StatusBadge";
import { showToast } from "../../redux/toast/toastSlice";

function BrandPage() {
    const [brands, setBrands] = useState([]);
    const [searchKeyword, setSearchKeyword] = useState("");
    const [isLoading, setIsLoading] = useState(true);

    const [pageNumber, setPageNumber] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(1);

    const dispatch = useDispatch();
    const navigate = useNavigate();
    useEffect(() => {
        loadBrands();
    }, [pageNumber]);

    async function loadBrands() {
        try {
            setIsLoading(true);

            const data = await getBrands(pageNumber, pageSize);

            const brandItems =
                data.items ||
                data.data ||
                data.results ||
                data.brands ||
                data;

            setBrands(Array.isArray(brandItems) ? brandItems : []);

            setTotalPages(data.totalPages || 1);
        } catch (error) {
            console.error("Failed to load brands:", error);

            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to load brands."
                })
            );
        } finally {
            setIsLoading(false);
        }
    }

    async function handleDeleteBrand(brandId) {
        const confirmed = window.confirm(
            "Are you sure you want to delete this brand?"
        );

        if (!confirmed) return;

        try {
            await deleteBrand(brandId);

            setBrands((prevBrands) =>
                prevBrands.filter((brand) => brand.brandId !== brandId)
            );

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Brand deleted successfully."
                })
            );
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to delete brand."
                })
            );
        }
    }

    const filteredBrands = brands.filter((brand) => {
        const keyword = searchKeyword.toLowerCase();

        return (
            brand.brandName?.toLowerCase().includes(keyword) ||
            brand.brandCode?.toLowerCase().includes(keyword)
        );
    });

    return (
        <div>
            <PageHeader
                title="Brands"
                description="Manage brand information and product associations."
                actionText="+ Add Brand"
                onActionClick={() => navigate("/brands/create")}
            />

            <div className="card border-0 shadow-sm">
                <div className="card-body">
                    <div className="d-flex justify-content-between align-items-center mb-3">
                        <h5 className="mb-0 fw-semibold">Brand List</h5>

                        <SearchBox
                            value={searchKeyword}
                            onChange={setSearchKeyword}
                            placeholder="Search brand..."
                        />
                    </div>

                    {isLoading ? (
                        <div className="text-center py-5 text-muted">
                            Loading brands...
                        </div>
                    ) : (
                        <>
                            <div className="table-responsive">
                                <table className="table table-hover align-middle">
                                    <thead className="table-light">
                                        <tr>
                                            <th>#</th>
                                            <th>Logo</th>
                                            <th>Brand Name</th>
                                            <th>Code</th>
                                            <th>Products</th>
                                            <th>Status</th>
                                            <th className="text-end">Actions</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        {filteredBrands.length === 0 ? (
                                            <tr>
                                                <td
                                                    colSpan="7"
                                                    className="text-center text-muted py-4"
                                                >
                                                    No brands found.
                                                </td>
                                            </tr>
                                        ) : (
                                            filteredBrands.map((brand, index) => (
                                                <tr key={brand.brandId}>
                                                    <td>
                                                        {(pageNumber - 1) * pageSize + index + 1}
                                                    </td>

                                                    <td>
                                                        {brand.brandImage ? (
                                                            <img
                                                                src={brand.brandImage}
                                                                alt={brand.brandName}
                                                                width="48"
                                                                height="48"
                                                                className="rounded object-fit-cover"
                                                            />
                                                        ) : (
                                                            <div
                                                                className="bg-light rounded d-flex align-items-center justify-content-center"
                                                                style={{
                                                                    width: "48px",
                                                                    height: "48px"
                                                                }}
                                                            >
                                                                <i className="bi bi-image text-muted"></i>
                                                            </div>
                                                        )}
                                                    </td>

                                                    <td className="fw-medium">
                                                        {brand.brandName}
                                                    </td>

                                                    <td>{brand.brandCode}</td>

                                                    <td>{brand.productCount}</td>

                                                    <td>
                                                        <StatusBadge
                                                            isActive={brand.isActive}
                                                        />
                                                    </td>

                                                    <td className="text-end">
                                                        <Link
                                                            to={`/brands/${brand.brandId}`}
                                                            className="btn btn-sm btn-outline-primary me-2"
                                                        >
                                                            View
                                                        </Link>

                                                        <Link
                                                            to={`/brands/${brand.brandId}/edit`}
                                                            className="btn btn-sm btn-outline-dark me-2"
                                                        >
                                                            Edit
                                                        </Link>

                                                        <button
                                                            type="button"
                                                            className="btn btn-sm btn-outline-danger"
                                                            onClick={() =>
                                                                handleDeleteBrand(brand.brandId)
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

                            {totalPages > 1 && (
                                <div className="d-flex justify-content-end align-items-center gap-2 mt-3">
                                    <button
                                        className="btn btn-sm btn-outline-secondary"
                                        disabled={pageNumber === 1}
                                        onClick={() => setPageNumber(pageNumber - 1)}
                                    >
                                        Previous
                                    </button>

                                    <span className="text-muted small">
                                        Page {pageNumber} of {totalPages}
                                    </span>

                                    <button
                                        className="btn btn-sm btn-outline-secondary"
                                        disabled={pageNumber === totalPages}
                                        onClick={() => setPageNumber(pageNumber + 1)}
                                    >
                                        Next
                                    </button>
                                </div>
                            )}
                        </>
                    )}
                </div>
            </div>
        </div>
    );
}

export default BrandPage;