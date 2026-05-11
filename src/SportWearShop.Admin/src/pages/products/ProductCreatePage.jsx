import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";

import { createProduct } from "../../api/productApi";
import { createProductVariant } from "../../api/productVariantApi";
import { createProductImage } from "../../api/productImageApi";
import { getBrands } from "../../api/brandApi";
import { getCategories } from "../../api/categoryApi";

import { showToast } from "../../redux/toast/toastSlice";

import { createProductRequestModel } from "../../models/productModel";
import { createProductVariantRequestModel } from "../../models/productVariantModel";
import { createProductImageRequestModel } from "../../models/productImageModel";

function getDefaultProductForm() {
    return {
        ...createProductRequestModel,
        status: "ACTIVE",
        brandId: "",
        categoryId: ""
    };
}

function getDefaultVariantForm() {
    return {
        ...createProductVariantRequestModel,
        status: "ACTIVE",
        salePrice: "",
        weightGrams: ""
    };
}

function getDefaultImageForm(sortOrder = 0) {
    return {
        ...createProductImageRequestModel,
        sortOrder,
        isPrimary: false
    };
}

function ProductCreatePage() {
    const navigate = useNavigate();
    const dispatch = useDispatch();

    const [step, setStep] = useState(1);
    const [createdProduct, setCreatedProduct] = useState(null);

    const [brands, setBrands] = useState([]);
    const [categories, setCategories] = useState([]);

    const [productForm, setProductForm] = useState(getDefaultProductForm());
    const [variantForm, setVariantForm] = useState(getDefaultVariantForm());
    const [imageForm, setImageForm] = useState(getDefaultImageForm());

    const [createdVariants, setCreatedVariants] = useState([]);
    const [createdImages, setCreatedImages] = useState([]);

    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        loadMasterData();
    }, []);

    async function loadMasterData() {
        try {
            const [brandData, categoryData] = await Promise.all([
                getBrands(),
                getCategories()
            ]);

            setBrands(brandData);
            setCategories(categoryData);
        } catch {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message: "Failed to load brand or category data."
                })
            );
        }
    }

    function handleProductChange(e) {
        const { name, value } = e.target;

        setProductForm((prev) => ({
            ...prev,
            [name]: value
        }));
    }

    function handleVariantChange(e) {
        const { name, value } = e.target;

        setVariantForm((prev) => ({
            ...prev,
            [name]: value
        }));
    }

    function handleImageChange(e) {
        const { name, value, type, checked } = e.target;

        setImageForm((prev) => ({
            ...prev,
            [name]: type === "checkbox" ? checked : value
        }));
    }

    async function handleCreateProduct(e) {
        e.preventDefault();

        const request = {
            ...productForm,
            brandId: Number(productForm.brandId),
            categoryId: Number(productForm.categoryId)
        };

        try {
            setIsSubmitting(true);

            const result = await createProduct(request);

            setCreatedProduct(result);
            setStep(2);

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

    async function handleCreateVariant(e) {
        e.preventDefault();

        if (!createdProduct) return;

        const request = {
            ...variantForm,
            listPrice: Number(variantForm.listPrice),
            salePrice:
                variantForm.salePrice === ""
                    ? null
                    : Number(variantForm.salePrice),
            weightGrams:
                variantForm.weightGrams === ""
                    ? null
                    : Number(variantForm.weightGrams),
            initialStockQuantity: Number(variantForm.initialStockQuantity)
        };

        try {
            setIsSubmitting(true);

            const result = await createProductVariant(
                createdProduct.productId,
                request
            );

            setCreatedVariants((prev) => [...prev, result]);
            setVariantForm(getDefaultVariantForm());

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Product variant created successfully."
                })
            );
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to create product variant."
                })
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    async function handleCreateImage(e) {
        e.preventDefault();

        if (!createdProduct) return;

        const request = {
            ...imageForm,
            productId: createdProduct.productId,
            productVariantId: null,
            sortOrder: Number(imageForm.sortOrder),
            isPrimary: Boolean(imageForm.isPrimary)
        };

        try {
            setIsSubmitting(true);

            const result = await createProductImage(request);

            setCreatedImages((prev) => {
                const updatedImages = [...prev, result];
                setImageForm(getDefaultImageForm(updatedImages.length));
                return updatedImages;
            });

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Product image created successfully."
                })
            );
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to create product image."
                })
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    function finishCreateProduct() {
        navigate(`/products/${createdProduct.productId}`);
    }

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">Add Product</h3>
                    <p className="text-muted mb-0">
                        Create product, variants and product images step by step.
                    </p>
                </div>

                <Link to="/products" className="btn btn-outline-secondary">
                    Back
                </Link>
            </div>

            <div className="card border-0 shadow-sm mb-4">
                <div className="card-body">
                    <div className="d-flex gap-3">
                        <StepItem number={1} title="Product" active={step === 1} completed={step > 1} />
                        <StepItem number={2} title="Variants" active={step === 2} completed={step > 2} />
                        <StepItem number={3} title="Images" active={step === 3} completed={false} />
                    </div>
                </div>
            </div>

            {step === 1 && (
                <ProductForm
                    productForm={productForm}
                    brands={brands}
                    categories={categories}
                    isSubmitting={isSubmitting}
                    onChange={handleProductChange}
                    onSubmit={handleCreateProduct}
                />
            )}

            {step === 2 && (
                <VariantForm
                    createdProduct={createdProduct}
                    variantForm={variantForm}
                    createdVariants={createdVariants}
                    isSubmitting={isSubmitting}
                    onChange={handleVariantChange}
                    onSubmit={handleCreateVariant}
                    onContinue={() => setStep(3)}
                />
            )}

            {step === 3 && (
                <ImageForm
                    imageForm={imageForm}
                    createdImages={createdImages}
                    isSubmitting={isSubmitting}
                    onChange={handleImageChange}
                    onSubmit={handleCreateImage}
                    onFinish={finishCreateProduct}
                />
            )}
        </div>
    );
}

function StepItem({ number, title, active, completed }) {
    return (
        <div
            className={
                active
                    ? "border rounded-3 px-3 py-2 bg-dark text-white"
                    : completed
                        ? "border rounded-3 px-3 py-2 bg-light text-dark"
                        : "border rounded-3 px-3 py-2 text-muted"
            }
        >
            <div className="small">Step {number}</div>
            <div className="fw-semibold">{title}</div>
        </div>
    );
}

export default ProductCreatePage;