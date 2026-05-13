import axiosClient from "./axiosClient";

export function buildBrandFormData(request, includeStatus = false) {
    const formData = new FormData();

    if (request.brandName) {
        formData.append("brandName", request.brandName);
    }

    if (request.brandCode) {
        formData.append("brandCode", request.brandCode);
    }

    if (request.brandImageFile) {
        formData.append("brandImageFile", request.brandImageFile);
    }

    if (
        includeStatus &&
        request.isActive !== undefined &&
        request.isActive !== null
    ) {
        formData.append("isActive", request.isActive);
    }

    return formData;
}

export async function getBrands(pageNumber = 1, pageSize = 10) {
    const response = await axiosClient.get("/brands", {
        params: {
            pageNumber,
            pageSize
        }
    });

    return response.data;
}


export async function getBrandById(brandId) {
    const response = await axiosClient.get(`/brands/${brandId}`);
    return response.data;
}

export async function createBrand(request) {
    const formData = buildBrandFormData(request);

    const response = await axiosClient.post(
        "/brands",
        formData,
        {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        }
    );

    return response.data;
}

export async function updateBrand(brandId, request) {
    const formData = buildBrandFormData(request, true);

    const response = await axiosClient.put(
        `/brands/${brandId}`,
        formData,
        {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        }
    );

    return response.data;
}

export async function deleteBrand(brandId) {
    await axiosClient.delete(`/brands/${brandId}`);
}