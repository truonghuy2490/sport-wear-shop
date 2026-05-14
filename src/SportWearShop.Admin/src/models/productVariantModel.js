// src/models/productVariantModels.js

export const productVariantResponseModel = {
    productVariantId: 0,
    sku: "",
    colorCode: "",
    colorName: "",
    sizeCode: "",
    sizeLabel: "",
    listPrice: 0,
    salePrice: null,
    weightGrams: null,
    status: "",
    quantityOnHand: 0,
    quantityReserved: 0,
    availableStock: 0,
    images: []
};

export const productVariantDetailResponseModel = {
    productVariantId: 0,
    productId: 0,
    productName: "",
    sku: "",
    colorCode: "",
    colorName: "",
    sizeCode: "",
    sizeLabel: "",
    listPrice: 0,
    salePrice: null,
    weightGrams: null,
    status: "",
    quantityOnHand: 0,
    quantityReserved: 0,
    images: []
};

export const createProductVariantRequestModel = {
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

export const updateProductVariantRequestModel = {
    sku: "",
    colorCode: "",
    colorName: "",
    sizeCode: "",
    sizeLabel: "",
    listPrice: 0,
    salePrice: null,
    weightGrams: null,
    status: "ACTIVE"
};


export const updateProductImageSortOrderRequestModel = {
    productImageId: 0,
    sortOrder: 0
};

export const updateProductImageSortOrdersRequestModel = {
    images: []
};

export const productVariantStatusOptions = [
    { value: "", label: "All statuses" },
    { value: 0, label: "Draft" },
    { value: 1, label: "Active" },
    { value: 3, label: "Deleted" }
];