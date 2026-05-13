import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useDispatch } from "react-redux";

import {
    getProductDetail,
    updateProduct
} from "../../api/productApi";

import {
    createProductVariants,
    updateProductVariant,
    updateProductVariantImageSortOrders
} from "../../api/productVariantApi";

import { createProductImage } from "../../api/productImageApi";
import { getBrands } from "../../api/brandApi";
import { getCategories } from "../../api/categoryApi";

import { createProductImageRequestModel } from "../../models/productImageModel";

import { showToast } from "../../redux/toast/toastSlice";

import StepIndicator from "../../components/products/StepIndicator";
import ProductForm from "../../components/products/ProductForm";
import VariantForm from "../../components/products/VariantForm";
import ImageStep from "../../components/products/ImageStep";

function ProductUpdatePage() {
    const genderMap = {
        Unisex: 0,
        Men: 1,
        Woman: 2,
        Kids: 3
    };

    const statusMap = {
        Active: "ACTIVE",
        Inactive: "INACTIVE"
    };
    const { productId } = useParams();
    const navigate = useNavigate();
    const dispatch = useDispatch();

    const [currentStep, setCurrentStep] = useState(1);
    const [isLoading, setIsLoading] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const [productDetail, setProductDetail] = useState(null);

    const [brands, setBrands] = useState([]);
    const [categories, setCategories] = useState([]);

    const [createdVariants, setCreatedVariants] = useState([]);
    const [selectedVariantId, setSelectedVariantId] = useState(null);
    const [uploadedImagesByVariantId, setUploadedImagesByVariantId] = useState({});

    const [productForm, setProductForm] = useState({
        productName: "",
        slug: "",
        brandId: "",
        categoryId: "",
        description: "",
        gender: "",
        baseMaterial: "",
        status: "ACTIVE"
    });

    const emptyVariantForm = {
        productVariantId: null,
        sku: "",
        colorCode: "",
        colorName: "",
        sizeCode: "",
        sizeLabel: "",
        listPrice: 0,
        salePrice: null,
        weightGrams: null,
        status: "ACTIVE",
        initialStockQuantity: 0,
        isExisting: false
    };

    const [variantForms, setVariantForms] = useState([]);
    const [imageForm, setImageForm] = useState({
        ...createProductImageRequestModel,
        isPrimary: false
    });

    useEffect(() => {
        loadInitialData();
    }, [productId]);

    async function loadInitialData() {
        try {
            setIsLoading(true);

            const [productResult, brandResult, categoryResult] =
                await Promise.all([
                    getProductDetail(productId),
                    getBrands(),
                    getCategories()
                ]);

            setProductDetail(productResult);
            setBrands(brandResult.items || brandResult);
            setCategories(categoryResult.items || categoryResult);

            setProductForm({
                productName: productResult.productName || "",
                slug: productResult.slug || "",
                brandId: productResult.brandId || "",
                categoryId: productResult.categoryId || "",
                description: productResult.description || "",
                gender: genderMap[productResult.gender] ?? "",
                baseMaterial: productResult.baseMaterial || "",
                status: statusMap[productResult.status] || "ACTIVE"
            });

            const mappedVariants = productResult.variants.map((variant) => ({
                productVariantId: variant.productVariantId,
                sku: variant.sku || "",
                colorCode: variant.colorCode || "",
                colorName: variant.colorName || "",
                sizeCode: variant.sizeCode || "",
                sizeLabel: variant.sizeLabel || "",
                listPrice: variant.listPrice || 0,
                salePrice: variant.salePrice,
                weightGrams: variant.weightGrams,
                status: statusMap[variant.status] || "ACTIVE",
                initialStockQuantity: variant.quantityOnHand || 0,
                isExisting: true
            }));
            setVariantForms(mappedVariants.length > 0 ? mappedVariants : [
                { ...emptyVariantForm }
            ]);

            setCreatedVariants(productResult.variants || []);

            const imagesByVariantId = {};

            productResult.variants.forEach((variant) => {
                imagesByVariantId[variant.productVariantId] =
                    variant.images || [];
            });

            setUploadedImagesByVariantId(imagesByVariantId);

            setSelectedVariantId(
                productResult.variants[0]?.productVariantId || null
            );
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to load product detail."
                })
            );
        } finally {
            setIsLoading(false);
        }
    }

    function updateProductForm(name, value) {
        setProductForm((prev) => ({
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

    function handleVariantChange(index, field, value) {
        setVariantForms((prev) =>
            prev.map((variant, i) =>
                i === index
                    ? { ...variant, [field]: value }
                    : variant
            )
        );
    }

    function handleAddVariant() {
        setVariantForms((prev) => [
            ...prev,
            { ...emptyVariantForm }
        ]);
    }

    function handleRemoveVariant(index) {
        setVariantForms((prev) =>
            prev.filter((_, i) => i !== index)
        );
    }

    function buildUpdateProductPayload() {
        return {
            brandId: Number(productForm.brandId),
            categoryId: Number(productForm.categoryId),
            productName: productForm.productName.trim(),
            description: productForm.description?.trim() || null,
            gender: Number(productForm.gender),
            baseMaterial: productForm.baseMaterial?.trim() || null,
            slug: productForm.slug?.trim() || null,
            status: productForm.status
        };
    }

    async function handleUpdateProduct(e) {
        e.preventDefault();

        try {
            setIsSubmitting(true);

            await updateProduct(productId, buildUpdateProductPayload());

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Product updated successfully."
                })
            );

            setCurrentStep(2);
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to update product."
                })
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    function buildVariantPayload(variant) {
        return {
            sku: variant.sku.trim(),
            colorCode: variant.colorCode.trim(),
            colorName: variant.colorName.trim(),
            sizeCode: variant.sizeCode.trim(),
            sizeLabel: variant.sizeLabel.trim(),
            listPrice: Number(variant.listPrice),
            salePrice:
                variant.salePrice === "" ||
                variant.salePrice === null ||
                variant.salePrice === undefined
                    ? null
                    : Number(variant.salePrice),
            weightGrams:
                variant.weightGrams === "" ||
                variant.weightGrams === null ||
                variant.weightGrams === undefined
                    ? null
                    : Number(variant.weightGrams),
            status: variant.status || "ACTIVE"
        };
    }

    function buildCreateVariantPayload(variant) {
        return {
            ...buildVariantPayload(variant),
            initialStockQuantity: Number(variant.initialStockQuantity || 0)
        };
    }

    async function handleSaveVariants(e) {
        e.preventDefault();

        try {
            setIsSubmitting(true);

            const existingVariants = variantForms.filter(
                (variant) => variant.isExisting
            );

            const newVariants = variantForms.filter(
                (variant) => !variant.isExisting
            );

            for (const variant of existingVariants) {
                await updateProductVariant(
                    variant.productVariantId,
                    buildVariantPayload(variant)
                );
            }

            let createdNewVariants = [];

            if (newVariants.length > 0) {
                createdNewVariants = await createProductVariants(
                    productId,
                    {
                        variants: newVariants.map(buildCreateVariantPayload)
                    }
                );
            }

            const nextVariants = [
                ...createdVariants,
                ...createdNewVariants
            ];

            setCreatedVariants(nextVariants);

            if (!selectedVariantId) {
                setSelectedVariantId(nextVariants[0]?.productVariantId || null);
            }

            dispatch(
                showToast({
                    type: "success",
                    title: "Success",
                    message: "Product variants saved successfully."
                })
            );

            setCurrentStep(3);
        } catch (error) {
            dispatch(
                showToast({
                    type: "error",
                    title: "Error",
                    message:
                        error.response?.data?.message ||
                        "Failed to save product variants."
                })
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    function buildCreateImagePayload() {
        return {
            productId: Number(productId),
            productVariantId: selectedVariantId,
            imageFile: imageForm.imageFile,
            altText: imageForm.altText?.trim() || null,
            isPrimary: imageForm.isPrimary
        };
    }

    async function handleCreateImage(e) {
        e.preventDefault();

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

    function handleImageSortOrderChange(productImageId, sortOrder) {
        setUploadedImagesByVariantId((prev) => {
            const currentImages = prev[selectedVariantId] || [];

            return {
                ...prev,
                [selectedVariantId]: currentImages.map((image) =>
                    image.productImageId === productImageId
                        ? {
                            ...image,
                            sortOrder: Number(sortOrder)
                        }
                        : image
                )
            };
        });
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
                sortOrder: Number(image.sortOrder ?? index + 1)
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

    function handleFinish() {
        navigate(`/products/${productId}`);
    }

    if (isLoading) {
        return <div>Loading product...</div>;
    }

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">Update Product</h3>
                    <p className="text-muted mb-0">
                        Update product information, variants and images.
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
                <>
                    <ProductForm
                        form={productForm}
                        brands={brands}
                        categories={categories}
                        onChange={updateProductForm}
                        onSubmit={handleUpdateProduct}
                        isSubmitting={isSubmitting}
                        submitText="Update Product"
                    />

                    <div className="d-flex justify-content-end mt-3">
                        <button
                            type="button"
                            className="btn btn-outline-dark"
                            onClick={() => setCurrentStep(2)}
                        >
                            Next: Variants
                        </button>
                    </div>
                </>
            )}

            {currentStep === 2 && (
                <>
                    <VariantForm
                        variants={variantForms}
                        product={productDetail}
                        onChange={handleVariantChange}
                        onAdd={handleAddVariant}
                        onRemove={handleRemoveVariant}
                        onSubmit={handleSaveVariants}
                        isSubmitting={isSubmitting}
                        submitText="Update Variants"
                    />

                    <div className="d-flex justify-content-between mt-3">
                        <button
                            type="button"
                            className="btn btn-outline-secondary"
                            onClick={() => setCurrentStep(1)}
                        >
                            Back: Product
                        </button>

                        <button
                            type="button"
                            className="btn btn-outline-dark"
                            onClick={() => setCurrentStep(3)}
                        >
                            Next: Images
                        </button>
                    </div>
                </>
            )}

            {currentStep === 3 && (
                <>
                    <ImageStep
                        form={imageForm}
                        product={productDetail}
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

                    <div className="d-flex justify-content-start mt-3">
                        <button
                            type="button"
                            className="btn btn-outline-secondary"
                            onClick={() => setCurrentStep(2)}
                        >
                            Back: Variants
                        </button>
                    </div>
                </>
            )}
        </div>
    );
}

export default ProductUpdatePage;