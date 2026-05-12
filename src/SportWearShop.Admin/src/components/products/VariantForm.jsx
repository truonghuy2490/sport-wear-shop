function VariantForm({
    variants,
    product,
    onChange,
    onAdd,
    onRemove,
    onSubmit,
    isSubmitting
}) {
    return (
        <form onSubmit={onSubmit}>
            <div className="card border-0 shadow-sm">
                <div className="card-body p-4">
                    <div className="d-flex justify-content-between align-items-start mb-3">
                        <div>
                            <h5 className="fw-semibold mb-1">
                                Step 2: Product variant information
                            </h5>

                            <p className="text-muted mb-0">
                                Product: {product?.productName}
                            </p>
                        </div>

                        <button
                            type="button"
                            className="btn btn-outline-dark btn-sm"
                            onClick={onAdd}
                        >
                            + Add Variant
                        </button>
                    </div>

                    {variants.map((form, index) => (
                        <div
                            key={index}
                            className="border rounded-3 p-3 mb-3"
                        >
                            <div className="d-flex justify-content-between mb-3">
                                <h6 className="fw-semibold mb-0">
                                    Variant #{index + 1}
                                </h6>

                                {variants.length > 1 && (
                                    <button
                                        type="button"
                                        className="btn btn-outline-danger btn-sm"
                                        onClick={() => onRemove(index)}
                                    >
                                        Remove
                                    </button>
                                )}
                            </div>

                            <div className="row g-3">
                                <div className="col-md-6">
                                    <label className="form-label">SKU</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={form.sku}
                                        onChange={(e) =>
                                            onChange(index, "sku", e.target.value)
                                        }
                                        required
                                    />
                                </div>

                                <div className="col-md-3">
                                    <label className="form-label">Color Code</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={form.colorCode}
                                        onChange={(e) =>
                                            onChange(index, "colorCode", e.target.value)
                                        }
                                        required
                                    />
                                </div>

                                <div className="col-md-3">
                                    <label className="form-label">Color Name</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={form.colorName}
                                        onChange={(e) =>
                                            onChange(index, "colorName", e.target.value)
                                        }
                                        required
                                    />
                                </div>

                                <div className="col-md-3">
                                    <label className="form-label">Size Code</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={form.sizeCode}
                                        onChange={(e) =>
                                            onChange(index, "sizeCode", e.target.value)
                                        }
                                        required
                                    />
                                </div>

                                <div className="col-md-3">
                                    <label className="form-label">Size Label</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={form.sizeLabel}
                                        onChange={(e) =>
                                            onChange(index, "sizeLabel", e.target.value)
                                        }
                                        required
                                    />
                                </div>

                                <div className="col-md-3">
                                    <label className="form-label">List Price</label>
                                    <input
                                        type="number"
                                        className="form-control"
                                        value={form.listPrice}
                                        onChange={(e) =>
                                            onChange(index, "listPrice", e.target.value)
                                        }
                                        min="0"
                                        required
                                    />
                                </div>

                                <div className="col-md-3">
                                    <label className="form-label">Sale Price</label>
                                    <input
                                        type="number"
                                        className="form-control"
                                        value={form.salePrice ?? ""}
                                        onChange={(e) =>
                                            onChange(index, "salePrice", e.target.value)
                                        }
                                        min="0"
                                        placeholder="Optional"
                                    />
                                </div>

                                <div className="col-md-3">
                                    <label className="form-label">Weight Grams</label>
                                    <input
                                        type="number"
                                        className="form-control"
                                        value={form.weightGrams ?? ""}
                                        onChange={(e) =>
                                            onChange(index, "weightGrams", e.target.value)
                                        }
                                        min="0"
                                    />
                                </div>

                                <div className="col-md-3">
                                    <label className="form-label">Initial Stock</label>
                                    <input
                                        type="number"
                                        className="form-control"
                                        value={form.initialStockQuantity}
                                        onChange={(e) =>
                                            onChange(index, "initialStockQuantity", e.target.value)
                                        }
                                        min="0"
                                        required
                                    />
                                </div>

                                <div className="col-md-3">
                                    <label className="form-label">Status</label>
                                    <select
                                        className="form-select"
                                        value={form.status}
                                        onChange={(e) =>
                                            onChange(index, "status", e.target.value)
                                        }
                                    >
                                        <option value="ACTIVE">ACTIVE</option>
                                        <option value="INACTIVE">INACTIVE</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                    ))}

                    <div className="d-flex justify-content-end mt-4">
                        <button
                            type="submit"
                            className="btn btn-dark"
                            disabled={isSubmitting}
                        >
                            {isSubmitting
                                ? "Creating..."
                                : "Create Variants"}
                        </button>
                    </div>
                </div>
            </div>
        </form>
    );
}

export default VariantForm;