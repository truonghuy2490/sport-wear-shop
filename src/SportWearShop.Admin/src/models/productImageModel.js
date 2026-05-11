// src/models/productImageModels.js

export const productImageResponseModel = {
    productImageId: 0,
    imageUrl: "",
    altText: "",
    sortOrder: 0,
    isPrimary: false
};

export const createProductImageRequestModel = {
    productId: 0,
    productVariantId: null,
    imageUrl: "",
    altText: "",
    sortOrder: 0,
    isPrimary: false
};

export const updateProductImageRequestModel = {
    imageUrl: "",
    altText: "",
    sortOrder: 0,
    isPrimary: false
};