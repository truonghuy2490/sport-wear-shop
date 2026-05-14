import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";

import {
    getStockByVariantId,
    getMovementsByVariantId,
    stockIn,
    stockOut
} from "../../api/inventoryApi";

import PageHeader from "../../components/common/PageHeader";
import { showToast } from "../../redux/toast/toastSlice";

function InventoryPage() {
    const dispatch = useDispatch();

    const { productVariantId: routeVariantId } = useParams();
    const navigate = useNavigate();

    const [productVariantId, setProductVariantId] = useState(
        routeVariantId || ""
    );
    const [stock, setStock] = useState(null);
    const [movements, setMovements] = useState([]);

    const [quantity, setQuantity] = useState(0);
    const [note, setNote] = useState("");

    const [isLoading, setIsLoading] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        if (routeVariantId) {
            loadInventory(routeVariantId);
        }
    }, [routeVariantId]);

    async function handleSearchStock(e) {
        e.preventDefault();

        if (!productVariantId) return;

        await loadInventory(productVariantId);
    }

    async function handleStockIn() {
        await handleStockMovement("stock-in");
    }

    async function handleStockOut() {
        await handleStockMovement("stock-out");
    }

    async function handleStockMovement(type) {
        if (!productVariantId || quantity <= 0) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message: "Please enter a valid quantity."
                })
            );

            return;
        }

        const request = {
            productVariantId: Number(productVariantId),
            quantity: Number(quantity),
            staffId: 1,
            note
        };

        try {
            setIsSubmitting(true);

            if (type === "stock-in") {
                await stockIn(request);
            } else {
                await stockOut(request);
            }

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message:
                        type === "stock-in"
                            ? "Stock in completed successfully."
                            : "Stock out completed successfully."
                })
            );

            setQuantity(0);
            setNote("");

           const [stockData, movementData] = await Promise.all([
                getStockByVariantId(productVariantId),
                getMovementsByVariantId(productVariantId)
            ]);

            setStock(stockData);

            setMovements(
                Array.isArray(movementData)
                    ? movementData
                    : movementData.items || movementData.data || []
            );
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to update stock."
                })
            );
        } finally {
            setIsSubmitting(false);
        }
    }


    async function loadInventory(variantId) {
        try {
            setIsLoading(true);

            const [stockData, movementData] = await Promise.all([
                getStockByVariantId(variantId),
                getMovementsByVariantId(variantId)
            ]);

            setStock(stockData);

            setMovements(
                Array.isArray(movementData)
                    ? movementData
                    : movementData.items || movementData.data || []
            );
        } catch (error) {
            setStock(null);
            setMovements([]);

            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to load inventory."
                })
            );
        } finally {
            setIsLoading(false);
        }
    }
    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <PageHeader
                    title="Inventory"
                    description="Manage product variant stock and movement history."
                />

                <button
                    type="button"
                    className="btn btn-outline-secondary"
                    onClick={() => navigate(-1)}
                >
                    Back
                </button>
            </div>

            <div className="card border-0 shadow-sm mb-4">
                <div className="card-body">
                    <form onSubmit={handleSearchStock}>
                        <div className="row g-3 align-items-end">
                            <div className="col-md-6">
                                <label className="form-label fw-medium">
                                    Product Variant ID
                                </label>

                                <input
                                    type="number"
                                    className="form-control"
                                    value={productVariantId}
                                    onChange={(e) =>
                                        setProductVariantId(e.target.value)
                                    }
                                    placeholder="Enter product variant ID"
                                />
                            </div>

                            {/* <div className="col-md-3">
                                <button
                                    type="submit"
                                    className="btn btn-dark w-100"
                                    disabled={isLoading}
                                >
                                    {isLoading ? "Loading..." : "Search"}
                                </button>
                            </div> */}
                        </div>
                    </form>
                </div>
            </div>

            {stock && (
                <>
                    <div className="row g-3 mb-4">
                        <div className="col-md-3">
                            <div className="card border-0 shadow-sm">
                                <div className="card-body">
                                    <div className="text-muted small">
                                        SKU
                                    </div>
                                    <h5 className="mb-0 fw-bold">
                                        {stock.sku}
                                    </h5>
                                </div>
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className="card border-0 shadow-sm">
                                <div className="card-body">
                                    <div className="text-muted small">
                                        Quantity On Hand
                                    </div>
                                    <h5 className="mb-0 fw-bold">
                                        {stock.quantityOnHand}
                                    </h5>
                                </div>
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className="card border-0 shadow-sm">
                                <div className="card-body">
                                    <div className="text-muted small">
                                        Reserved
                                    </div>
                                    <h5 className="mb-0 fw-bold">
                                        {stock.quantityReserved}
                                    </h5>
                                </div>
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className="card border-0 shadow-sm">
                                <div className="card-body">
                                    <div className="text-muted small">
                                        Available Stock
                                    </div>
                                    <h5 className="mb-0 fw-bold">
                                        {stock.availableStock}
                                    </h5>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="card border-0 shadow-sm mb-4">
                        <div className="card-body">
                            <h5 className="fw-semibold mb-3">
                                Stock Adjustment
                            </h5>

                            <div className="row g-3">
                                <div className="col-md-3">
                                    <label className="form-label fw-medium">
                                        Quantity
                                    </label>

                                    <input
                                        type="number"
                                        className="form-control"
                                        value={quantity}
                                        onChange={(e) =>
                                            setQuantity(e.target.value)
                                        }
                                        min="1"
                                    />
                                </div>

                                <div className="col-md-6">
                                    <label className="form-label fw-medium">
                                        Note
                                    </label>

                                    <input
                                        type="text"
                                        className="form-control"
                                        value={note}
                                        onChange={(e) =>
                                            setNote(e.target.value)
                                        }
                                        placeholder="Optional note"
                                    />
                                </div>

                                <div className="col-md-3 d-flex align-items-end gap-2">
                                    <button
                                        type="button"
                                        className="btn btn-success w-50"
                                        onClick={handleStockIn}
                                        disabled={isSubmitting}
                                    >
                                        Stock In
                                    </button>

                                    <button
                                        type="button"
                                        className="btn btn-outline-danger w-50"
                                        onClick={handleStockOut}
                                        disabled={isSubmitting}
                                    >
                                        Stock Out
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="card border-0 shadow-sm">
                        <div className="card-body">
                            <h5 className="fw-semibold mb-3">
                                Movement History
                            </h5>

                            <div className="table-responsive">
                                <table className="table table-hover align-middle">
                                    <thead className="table-light">
                                        <tr>
                                            <th>#</th>
                                            <th>Type</th>
                                            <th>SKU</th>
                                            <th>Quantity</th>
                                            <th>Reference</th>
                                            <th>Note</th>
                                            <th>Created At</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        {movements.length === 0 ? (
                                            <tr>
                                                <td
                                                    colSpan="7"
                                                    className="text-center text-muted py-4"
                                                >
                                                    No movements found.
                                                </td>
                                            </tr>
                                        ) : (
                                            movements.map((movement, index) => (
                                                <tr
                                                    key={
                                                        movement.inventoryMovementId
                                                    }
                                                >
                                                    <td>{index + 1}</td>

                                                    <td>
                                                        <span
                                                            className={
                                                                movement.movementType ===
                                                                "StockIn"
                                                                    ? "badge bg-success"
                                                                    : "badge bg-danger"
                                                            }
                                                        >
                                                            {
                                                                movement.movementType
                                                            }
                                                        </span>
                                                    </td>

                                                    <td>{movement.sku}</td>

                                                    <td>
                                                        {movement.quantity}
                                                    </td>

                                                    <td>
                                                        {movement.referenceType ||
                                                            "-"}
                                                        {movement.referenceId
                                                            ? ` #${movement.referenceId}`
                                                            : ""}
                                                    </td>

                                                    <td>
                                                        {movement.note || "-"}
                                                    </td>
                                                            
                                                    <td>
                                                        {new Date(
                                                            `${movement.createdAtUtc}Z`
                                                        ).toLocaleString()}
                                                    </td>
                                                </tr>
                                            ))
                                        )}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
}

export default InventoryPage;