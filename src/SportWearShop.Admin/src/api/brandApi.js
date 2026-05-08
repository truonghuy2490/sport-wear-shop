import axiosClient from "./axiosClient";

export async function getBrands() {
    const response = await axiosClient.get("/brands");
    return response.data;
}

export async function getBrandById(brandId) {
    const response = await axiosClient.get(`/brands/${brandId}`);
    return response.data;
}

export async function createBrand(request) {
    const response = await axiosClient.post("/brands", request);
    return response.data;
}

export async function updateBrand(brandId, request) {
    const response = await axiosClient.put(`/brands/${brandId}`, request);
    return response.data;
}

export async function deleteBrand(brandId) {
    await axiosClient.delete(`/brands/${brandId}`);
}