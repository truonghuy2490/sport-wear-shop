function ProductForm({
    form,
    brands,
    categories,
    onChange,
    onSubmit,
    isSubmitting,
    submitText = "Create Product"
}) {
    return (
        <form onSubmit={onSubmit}>
            <div className="card border-0 shadow-sm">
                <div className="card-body p-4">
                    <h5 className="fw-semibold mb-3">
                        Step 1: Product basic information
                    </h5>

                    <div className="row g-3">
                        <div className="col-md-6">
                            <label className="form-label">Product Name</label>
                            <input
                                type="text"
                                className="form-control"
                                value={form.productName}
                                onChange={(e) =>
                                    onChange("productName", e.target.value)
                                }
                                required
                            />
                        </div>

                        <div className="col-md-6">
                            <label className="form-label">Slug</label>
                            <input
                                type="text"
                                className="form-control"
                                value={form.slug || ""}
                                onChange={(e) =>
                                    onChange("slug", e.target.value)
                                }
                                placeholder="Optional"
                            />
                        </div>

                        <div className="col-md-6">
                            <label className="form-label">Brand</label>

                            <select
                                className="form-select"
                                value={form.brandId || ""}
                                onChange={(e) => onChange("brandId", e.target.value)}
                                required
                            >
                                <option value="">Select brand</option>

                                {brands.map((brand) => (
                                    <option
                                        key={brand.brandId}
                                        value={brand.brandId}
                                    >
                                        {brand.brandName}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="col-md-6">
                            <label className="form-label">Category</label>

                            <select
                                className="form-select"
                                value={form.categoryId || ""}
                                onChange={(e) => onChange("categoryId", e.target.value)}
                                required
                            >
                                <option value="">Select category</option>

                                {categories.map((category) => (
                                    <option
                                        key={category.categoryId}
                                        value={category.categoryId}
                                    >
                                        {category.categoryName}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="col-md-6">
                            <label className="form-label">Gender</label>
                            <select
                                className="form-select"
                                value={form.gender}
                                onChange={(e) =>
                                    onChange("gender", e.target.value)
                                }
                                required
                            >
                                <option value="">Select gender</option>
                                <option value="0">Unisex</option>
                                <option value="1">Men</option>
                                <option value="2">Woman</option>
                                <option value="3">Kids</option>
                            </select>
                        </div>

                        <div className="col-md-6">
                            <label className="form-label">Base Material</label>
                            <input
                                type="text"
                                className="form-control"
                                value={form.baseMaterial || ""}
                                onChange={(e) =>
                                    onChange("baseMaterial", e.target.value)
                                }
                                placeholder="Optional"
                            />
                        </div>

                        <div className="col-md-12">
                            <label className="form-label">Description</label>
                            <textarea
                                className="form-control"
                                rows="4"
                                value={form.description || ""}
                                onChange={(e) =>
                                    onChange("description", e.target.value)
                                }
                                placeholder="Optional"
                            />
                        </div>
                    </div>

                    <div className="d-flex justify-content-end mt-4">
                        <button
                            type="submit"
                            className="btn btn-dark"
                            disabled={isSubmitting}
                        >
                            {isSubmitting ? "Saving..." : submitText}
                        </button>
                    </div>
                </div>
            </div>
        </form>
    );
}

export default ProductForm;