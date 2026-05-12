import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";

import { createProduct } from "../../api/productApi";
import { createProductVariants, updateProductVariantImageSortOrders } from "../../api/productVariantApi";
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

import StepIndicator from "../../components/products/StepIndicator";
import ProductForm from "../../components/products/ProductForm";
import VariantForm from "../../components/products/VariantForm";
import ImageStep from "../../components/products/ImageStep";


function ProductCreatePage() {
    const navigate = useNavigate();
    const dispatch = useDispatch();

    const [currentStep, setCurrentStep] = useState(1);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const [createdProduct, setCreatedProduct] = useState(null);
    const [createdVariants, setCreatedVariants] = useState([]);
    const [selectedVariantId, setSelectedVariantId] = useState(null);
    const [uploadedImages, setUploadedImages] = useState([]);
    const [uploadedImagesByVariantId, setUploadedImagesByVariantId] = useState({});

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
            productVariantId: selectedVariantId,
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

        setSelectedVariantId(result[0]?.productVariantId || null);
        setCurrentStep(3);
    }

    async function handleUpdateImageSortOrders() {
        if (!selectedVariantId) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message: "Please select a variant first."
                })
            );
            return;
        }

        const currentImages = uploadedImagesByVariantId[selectedVariantId] || [];

        if (currentImages.length === 0) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message: "No images to update."
                })
            );
            return;
        }

        const request = {
            images: currentImages.map((image, index) => ({
                productImageId: image.productImageId,
                sortOrder: Number(image.sortOrder || index + 1)
            }))
        };

        try {
            setIsSubmitting(true);

            await updateProductVariantImageSortOrders(
                selectedVariantId,
                request
            );

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Image sort orders updated successfully."
                })
            );
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to update image sort orders."
                })
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    function handleImageSortOrderChange(productImageId, sortOrder) {
        setUploadedImagesByVariantId((prev) => {
            const currentImages = prev[selectedVariantId] || [];

            return {
                ...prev,
                [selectedVariantId]: currentImages.map((image) =>
                    image.productImageId === productImageId
                        ? { ...image, sortOrder: Number(sortOrder) }
                        : image
                )
            };
        });
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

        if (!selectedVariantId) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message: "Please select a variant first."
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

            const result = await createProductImage(buildCreateImagePayload());
            setUploadedImagesByVariantId((prev) => ({
                ...prev,
                [selectedVariantId]: [
                    ...(prev[selectedVariantId] || []),
                    result
                ]
            }));

            setUploadedImages((prev) => [...prev, result]);

            setImageForm({
                ...createProductImageRequestModel,
                isPrimary: false
            });

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Product image uploaded successfully."
                })
            );
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
                    variants={createdVariants}
                    selectedVariantId={selectedVariantId}
                    uploadedImages={uploadedImagesByVariantId[selectedVariantId] || []}
                    onSelectVariant={setSelectedVariantId}
                    onChange={updateImageForm}
                    onSubmit={handleCreateImage}
                    onSortOrderChange={handleImageSortOrderChange}
                    onUpdateSortOrders={handleUpdateImageSortOrders}
                    onFinish={handleFinish}
                    isSubmitting={isSubmitting}
                />
            )}
        </div>
    );
}

export default ProductCreatePage;