export const createBrandRequestModel = {
    brandName: "",
    brandCode: "",
    brandImageFile: null
};

export const updateBrandRequestModel = {
    brandName: "",
    brandCode: "",
    brandImageFile: null,
    isActive: true
};

export const brandResponseModel = {
    brandId: 0,
    brandName: "",
    brandCode: "",
    brandImage: "",
    isActive: true,
    productCount: 0
};

export const brandDetailResponseModel = {
    brandId: 0,
    brandName: "",
    brandCode: "",
    brandImage: "",
    isActive: true,
    createdAtUtc: "",
    updatedAtUtc: "",
    productCount: 0
};