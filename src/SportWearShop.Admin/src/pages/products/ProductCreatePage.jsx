import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";

import { createProduct } from "../../api/productApi";
import { createProductVariants } from "../../api/productVariantApi";
import { getBrands } from "../../api/brandApi";
import { getCategories } from "../../api/categoryApi";
import { createProductImage } from "../../api/productImageApi";

import {
    createProductImageRequestModel
} from "../../models/productImageModel";
import {
    createProductRequestModel
} from "../../models/productModel";

import {
    createProductVariantRequestModel
} from "../../models/productVariantModel";

import { showToast } from "../../redux/toast/toastSlice";

function ProductCreatePage() {
    const navigate = useNavigate();
    const dispatch = useDispatch();

    const [currentStep, setCurrentStep] = useState(1);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const [createdProduct, setCreatedProduct] = useState(null);
    const [createdVariant, setCreatedVariant] = useState(null);

    const [brands, setBrands] = useState([]);
    const [categories, setCategories] = useState([]);

    const emptyVariantForm = {
        sku: "",
        colorCode: "",
        colorName: "",
        sizeCode: "",
        sizeLabel: "",
        listPrice: 0,
        salePrice: null,
        weightGrams: null,
        status: "ACTIVE",
        initialStockQuantity: 0
    };

    const [variantForms, setVariantForms] = useState([
        { ...emptyVariantForm }
    ]);

    const [productForm, setProductForm] = useState({
        ...createProductRequestModel,
        productCode: undefined,
        status: undefined
    });

    const [variantForm, setVariantForm] = useState({
        ...createProductVariantRequestModel
    });

    const [imageForm, setImageForm] = useState({
        ...createProductImageRequestModel,
        isPrimary: true
    });

    useEffect(() => {
        loadDropdownData();
    }, []);

    async function loadDropdownData() {
        try {
            const [brandResult, categoryResult] = await Promise.all([
                getBrands(),
                getCategories()
            ]);

            setBrands(brandResult.items || brandResult);
            setCategories(categoryResult.items || categoryResult);
        } catch {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message: "Failed to load brands and categories."
                })
            );
        }
    }

    function updateProductForm(name, value) {
        setProductForm((prev) => ({
            ...prev,
            [name]: value
        }));
    }

    function updateVariantForm(name, value) {
        setVariantForm((prev) => ({
            ...prev,
            [name]: value
        }));
    }

    function updateImageForm(name, value) {
        setImageForm((prev) => ({
            ...prev,
            [name]: value
        }));
    }

    function buildCreateProductPayload() {
        return {
            brandId: Number(productForm.brandId),
            categoryId: Number(productForm.categoryId),
            productName: productForm.productName.trim(),
            description: productForm.description?.trim() || null,
            gender: Number(productForm.gender),
            baseMaterial: productForm.baseMaterial?.trim() || null,
            slug: productForm.slug?.trim() || null
        };
    }

    function buildCreateVariantPayload() {
        return {
            sku: variantForm.sku.trim(),
            colorCode: variantForm.colorCode.trim(),
            colorName: variantForm.colorName.trim(),
            sizeCode: variantForm.sizeCode.trim(),
            sizeLabel: variantForm.sizeLabel.trim(),
            listPrice: Number(variantForm.listPrice),
            salePrice:
                variantForm.salePrice === "" ||
                variantForm.salePrice === null ||
                variantForm.salePrice === undefined
                    ? null
                    : Number(variantForm.salePrice),
            weightGrams:
                variantForm.weightGrams === "" ||
                variantForm.weightGrams === null ||
                variantForm.weightGrams === undefined
                    ? null
                    : Number(variantForm.weightGrams),
            status: variantForm.status || "ACTIVE",
            initialStockQuantity: Number(variantForm.initialStockQuantity || 0)
        };
    }

    function buildCreateImagePayload() {
        return {
            productId: createdProduct.productId,
            productVariantId: createdVariant?.productVariantId || null,
            imageFile: imageForm.imageFile,
            altText: imageForm.altText?.trim() || null,
            isPrimary: imageForm.isPrimary
        };
    }

    function handleVariantChange(index, field, value) {
        setVariantForms(prev =>
            prev.map((variant, i) =>
                i === index
                    ? { ...variant, [field]: value }
                    : variant
            )
        );
    }

    function handleAddVariant() {
        setVariantForms(prev => [
            ...prev,
            { ...emptyVariantForm }
        ]);
    }

    function handleRemoveVariant(index) {
        setVariantForms(prev =>
            prev.filter((_, i) => i !== index)
        );
    }

    async function handleCreateProduct(e) {
        e.preventDefault();

        try {
            setIsSubmitting(true);

            const payload = buildCreateProductPayload();
            const result = await createProduct(payload);

            setCreatedProduct(result);
            setCurrentStep(2);

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Product created successfully."
                })
            );
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to create product."
                })
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    async function handleCreateVariants(e) {
        e.preventDefault();

        const request = {
            variants: variantForms.map(variant => ({
                sku: variant.sku.trim(),
                colorCode: variant.colorCode.trim(),
                colorName: variant.colorName.trim(),
                sizeCode: variant.sizeCode.trim(),
                sizeLabel: variant.sizeLabel.trim(),
                listPrice: Number(variant.listPrice),
                salePrice:
                    variant.salePrice === "" || variant.salePrice === null
                        ? null
                        : Number(variant.salePrice),
                weightGrams:
                    variant.weightGrams === "" || variant.weightGrams === null
                        ? null
                        : Number(variant.weightGrams),
                status: variant.status,
                initialStockQuantity: Number(variant.initialStockQuantity)
            }))
        };

        const result = await createProductVariants(
            createdProduct.productId,
            request
        );

        setCreatedVariants(result);

        dispatch(
            showToast({
                type: "success",
                title: "Success",
                message: "Product variants created successfully."
            })
        );

        setCurrentStep(3);
    }

    async function handleCreateImage(e) {
        e.preventDefault();

        if (!createdProduct?.productId) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message: "Please create product first."
                })
            );
            return;
        }

        if (!imageForm.imageFile) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message: "Please select an image."
                })
            );
            return;
        }

        try {
            setIsSubmitting(true);

            await createProductImage(buildCreateImagePayload());

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Product image uploaded successfully."
                })
            );

            navigate(`/products/${createdProduct.productId}`);
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to upload product image."
                })
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    function handleFinish() {
        navigate(`/products/${createdProduct.productId}`);
    }

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">Create Product</h3>
                    <p className="text-muted mb-0">
                        Create product, variant and image step by step.
                    </p>
                </div>

                <button
                    type="button"
                    className="btn btn-outline-secondary"
                    onClick={() => navigate("/products")}
                >
                    Back
                </button>
            </div>

            <StepIndicator currentStep={currentStep} />

            {currentStep === 1 && (
                <ProductForm
                    form={productForm}
                    brands={brands}
                    categories={categories}
                    onChange={updateProductForm}
                    onSubmit={handleCreateProduct}
                    isSubmitting={isSubmitting}
                />
            )}

            {currentStep === 2 && (
                <VariantForm
                    variants={variantForms}
                    product={createdProduct}
                    onChange={handleVariantChange}
                    onAdd={handleAddVariant}
                    onRemove={handleRemoveVariant}
                    onSubmit={handleCreateVariants}
                    isSubmitting={isSubmitting}
                />
            )}

            {currentStep === 3 && (
                <ImageStep
                    form={imageForm}
                    product={createdProduct}
                    variant={createdVariant}
                    onChange={updateImageForm}
                    onSubmit={handleCreateImage}
                    onFinish={handleFinish}
                    isSubmitting={isSubmitting}
                />
            )}
        </div>
    );
}

function StepIndicator({ currentStep }) {
    const steps = [
        { number: 1, label: "Product" },
        { number: 2, label: "Variant" },
        { number: 3, label: "Image" }
    ];

    return (
        <div className="card border-0 shadow-sm mb-4">
            <div className="card-body">
                <div className="d-flex justify-content-between">
                    {steps.map((step) => (
                        <div
                            key={step.number}
                            className="d-flex align-items-center gap-2"
                        >
                            <span
                                className={
                                    currentStep >= step.number
                                        ? "badge rounded-pill bg-dark"
                                        : "badge rounded-pill bg-secondary"
                                }
                            >
                                {step.number}
                            </span>

                            <span
                                className={
                                    currentStep === step.number
                                        ? "fw-semibold"
                                        : "text-muted"
                                }
                            >
                                {step.label}
                            </span>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}

function ProductForm({
    form,
    brands,
    categories,
    onChange,
    onSubmit,
    isSubmitting
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
                            {isSubmitting ? "Creating..." : "Create Product"}
                        </button>
                    </div>
                </div>
            </div>
        </form>
    );
}

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

function ImageStep({
    form,
    product,
    variant,
    onChange,
    onSubmit,
    onFinish,
    isSubmitting
}) {
    return (
        <form onSubmit={onSubmit}>
            <div className="card border-0 shadow-sm">
                <div className="card-body p-4">
                    <h5 className="fw-semibold mb-1">
                        Step 3: Product image information
                    </h5>

                    <p className="text-muted mb-4">
                        Upload image for created product and variant.
                    </p>

                    <div className="row g-3 mb-4">
                        <div className="col-md-6">
                            <div className="border rounded-3 p-3">
                                <div className="text-muted small">Product</div>
                                <div className="fw-semibold">
                                    {product?.productName}
                                </div>
                                <div className="text-muted small">
                                    ID: {product?.productId}
                                </div>
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="border rounded-3 p-3">
                                <div className="text-muted small">Variant</div>
                                <div className="fw-semibold">
                                    {variant?.sku}
                                </div>
                                <div className="text-muted small">
                                    ID: {variant?.productVariantId}
                                </div>
                            </div>
                        </div>
                    </div>

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

                    {form.imageFile && (
                        <div className="mt-4">
                            <div className="text-muted small mb-2">
                                Preview
                            </div>

                            <img
                                src={URL.createObjectURL(form.imageFile)}
                                alt="Preview"
                                className="rounded border"
                                style={{
                                    width: "180px",
                                    height: "180px",
                                    objectFit: "cover"
                                }}
                            />
                        </div>
                    )}

                    <div className="d-flex justify-content-between mt-4">
                        <button
                            type="button"
                            className="btn btn-outline-secondary"
                            onClick={onFinish}
                        >
                            Skip Image
                        </button>

                        <button
                            type="submit"
                            className="btn btn-dark"
                            disabled={isSubmitting}
                        >
                            {isSubmitting ? "Uploading..." : "Upload Image"}
                        </button>
                    </div>
                </div>
            </div>
        </form>
    );
}

export default ProductCreatePage;