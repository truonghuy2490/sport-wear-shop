// src/models/productModels.js

export const productResponseModel = {
    productId: 0,
    productName: "",
    productCode: "",
    slug: "",
    brandName: "",
    categoryName: "",
    gender: "",
    status: "",
    thumbnailUrl: "",
    minPrice: 0,
    minSalePrice: null,
    totalVariants: 0
};

export const productDetailResponseModel = {
    productId: 0,
    productName: "",
    productCode: "",
    slug: "",
    description: "",
    gender: "",
    baseMaterial: "",
    status: "",
    brandName: "",
    categoryName: "",
    createdAtUtc: "",
    updatedAtUtc: "",
    images: [],
    variants: []
};

export const createProductRequestModel = {
    productName: "",
    productCode: "",
    slug: "",
    description: "",
    gender: "",
    baseMaterial: "",
    status: "",
    brandId: 0,
    categoryId: 0
};

export const updateProductRequestModel = {
    productName: "",
    productCode: "",
    slug: "",
    description: "",
    gender: "",
    baseMaterial: "",
    status: "",
    brandId: null,
    categoryId: null
};

export const productQueryModel = {
    keyword: "",
    brandId: null,
    categoryId: null,
    gender: "",
    status: "",
    minPrice: null,
    maxPrice: null,
    pageNumber: 1,
    pageSize: 10
};

export const updateStatusRequestModel = {
    status: ""
};