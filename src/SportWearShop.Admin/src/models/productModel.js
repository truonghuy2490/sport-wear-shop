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

    brandId: 0,
    brandName: "",

    categoryId: 0,
    categoryName: "",

    status: "",
    createdAtUtc: "",
    updatedAtUtc: "",

    images: [],
    variants: []
};

export const createProductRequestModel = {
    brandId: 0,
    categoryId: 0,
    productName: "",
    description: "",
    gender: "",
    baseMaterial: "",
    slug: ""
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

export const productQueryRequestModel = {
    pageNumber: 1,
    pageSize: 10,

    searchTerm: "",

    status: "",

    sortBy: 1,
    isAscending: false
};

export const productStatusOptions = [
    { value: "", label: "All statuses" },
    { value: 0, label: "Draft" },
    { value: 1, label: "Active" },
    { value: 2, label: "Deleted" }
];

export const productSortByOptions = [
    { value: 1, label: "Created Date" },
    { value: 2, label: "Updated Date" },
    { value: 3, label: "Product Name" },
    { value: 4, label: "Product Code" },
    { value: 5, label: "Brand" },
    { value: 6, label: "Category" },
    { value: 7, label: "Price" }
];