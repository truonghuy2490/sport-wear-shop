export const categoryResponseModel = {
    categoryId: 0,
    parentCategoryId: null,
    categoryName: "",
    categoryCode: "",
    description: "",
    sortOrder: 0,
    isActive: true
};

export const categoryDetailResponseModel = {
    categoryId: 0,
    parentCategoryId: null,
    categoryName: "",
    categoryCode: "",
    description: "",
    sortOrder: 0,
    isActive: true,
    children: []
};

export const createCategoryRequestModel = {
    parentCategoryId: null,
    categoryName: "",
    categoryCode: "",
    description: "",
    sortOrder: 0
};

export const updateCategoryRequestModel = {
    parentCategoryId: null,
    categoryName: "",
    categoryCode: "",
    description: "",
    sortOrder: 0,
    isActive: true
};