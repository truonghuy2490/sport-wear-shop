import axiosClient from "./axiosClient";

export async function getProducts(pageNumber = 1, pageSize = 10) {
    const response = await axiosClient.get("/products", {
        params: {
            pageNumber,
            pageSize
        }
    });

    return response.data;
}

export async function getProductDetail(productId) {
    const response = await axiosClient.get(`/products/${productId}`);
    return response.data;
}

export async function createProduct(request) {
    const response = await axiosClient.post("/products", request);
    return response.data;
}

export async function updateProduct(productId, request) {
    const response = await axiosClient.put(`/products/${productId}`, request);
    return response.data;
}

export async function deleteProduct(productId) {
    await axiosClient.delete(`/products/${productId}`);
}