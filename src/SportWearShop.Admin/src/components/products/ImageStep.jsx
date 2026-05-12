function ImageStep({
    form,
    product,
    variants,
    selectedVariantId,
    uploadedImages,
    onSelectVariant,
    onChange,
    onSubmit,
    onSortOrderChange,
    onUpdateSortOrders,
    onFinish,
    isSubmitting
}) {
    const selectedVariant = variants.find(
        (variant) => variant.productVariantId === selectedVariantId
    );

    return (
        <div className="row g-4">
            <div className="col-md-3">
                <div className="card border-0 shadow-sm h-100">
                    <div className="card-body p-3">
                        <h6 className="fw-semibold mb-3">Product Variants</h6>

                        {variants.map((variant) => (
                            <button
                                key={variant.productVariantId}
                                type="button"
                                className={
                                    selectedVariantId === variant.productVariantId
                                        ? "btn btn-dark w-100 text-start mb-2"
                                        : "btn btn-outline-secondary w-100 text-start mb-2"
                                }
                                onClick={() => {
                                    onSelectVariant(variant.productVariantId);
                                    onChange("imageFile", null);
                                    onChange("altText", "");
                                    onChange("isPrimary", false);
                                }}
                            >
                                <div className="fw-semibold">{variant.sku}</div>
                                <div className="small">
                                    {variant.colorName} / {variant.sizeLabel}
                                </div>
                            </button>
                        ))}
                    </div>
                </div>
            </div>

            <div className="col-md-9">
                <div className="card border-0 shadow-sm">
                    <div className="card-body p-4">
                        <h5 className="fw-semibold mb-1">
                            Step 3: Product images
                        </h5>

                        <p className="text-muted mb-4">
                            Upload images for selected variant.
                        </p>

                        <div className="border rounded-3 p-3 mb-4">
                            <div className="text-muted small">Selected Variant</div>
                            <div className="fw-semibold">
                                {selectedVariant?.sku}
                            </div>
                            <div className="text-muted small">
                                {selectedVariant?.colorName} / {selectedVariant?.sizeLabel}
                            </div>
                        </div>

                        <form onSubmit={onSubmit}>
                            <div className="row g-3">
                                <div className="col-md-6">
                                    <label className="form-label">Image File</label>
                                    <input
                                        type="file"
                                        className="form-control"
                                        accept="image/*"
                                        onChange={(e) =>
                                            onChange("imageFile", e.target.files[0])
                                        }
                                        required
                                    />
                                </div>

                                <div className="col-md-6">
                                    <label className="form-label">Alt Text</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={form.altText || ""}
                                        onChange={(e) =>
                                            onChange("altText", e.target.value)
                                        }
                                        placeholder="Optional"
                                    />
                                </div>

                                <div className="col-md-12">
                                    <div className="form-check">
                                        <input
                                            type="checkbox"
                                            className="form-check-input"
                                            id="isPrimary"
                                            checked={form.isPrimary}
                                            onChange={(e) =>
                                                onChange("isPrimary", e.target.checked)
                                            }
                                        />

                                        <label
                                            className="form-check-label"
                                            htmlFor="isPrimary"
                                        >
                                            Set as primary image
                                        </label>
                                    </div>
                                </div>
                            </div>

                            <div className="d-flex justify-content-end mt-4">
                                <button
                                    type="submit"
                                    className="btn btn-dark"
                                    disabled={isSubmitting || !selectedVariantId}
                                >
                                    {isSubmitting ? "Uploading..." : "Add Image"}
                                </button>
                            </div>
                        </form>

                        <hr className="my-4" />

                        <h6 className="fw-semibold mb-3">Uploaded Images</h6>

                        {uploadedImages.length === 0 ? (
                            <div className="text-muted">
                                No images uploaded for this variant yet.
                            </div>
                        ) : (
                            <div className="row g-3">
                                {uploadedImages.map((image, index) => (
                                    <div key={image.productImageId} className="col-md-3">
                                        <div className="border rounded-3 p-2">
                                            <img
                                                src={image.imageUrl}
                                                alt={image.altText || ""}
                                                className="w-100 rounded"
                                                style={{
                                                    height: "120px",
                                                    objectFit: "cover"
                                                }}
                                            />

                                            <div className="mt-2">
                                                <label className="form-label small mb-1">
                                                    Sort Order
                                                </label>

                                                <input
                                                    type="number"
                                                    className="form-control form-control-sm"
                                                    min="1"
                                                    value={image.sortOrder ?? index + 1}
                                                    onChange={(e) =>
                                                        onSortOrderChange(
                                                            image.productImageId,
                                                            e.target.value
                                                        )
                                                    }
                                                />
                                            </div>

                                            {image.isPrimary && (
                                                <span className="badge bg-dark">
                                                    Primary
                                                </span>
                                            )}
                                        </div>
                                    </div>
                                ))}
                                {uploadedImages.length > 0 && (
                                    <div className="d-flex justify-content-end mb-3">
                                        <button
                                            type="button"
                                            className="btn btn-outline-dark btn-sm"
                                            onClick={onUpdateSortOrders}
                                            disabled={isSubmitting}
                                        >
                                            {isSubmitting ? "Updating..." : "Update Sort Order"}
                                        </button>
                                    </div>
                                )}
                                <div className="d-flex justify-content-end mt-4">
                                    <button
                                        type="button"
                                        className="btn btn-outline-secondary"
                                        onClick={onFinish}
                                    >
                                        Finish
                                    </button>
                                </div>
                            </div>
                        )}

                        
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ImageStep;