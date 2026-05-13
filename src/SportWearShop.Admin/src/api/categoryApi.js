import axiosClient from "./axiosClient";

export async function getCategories() {
    const response = await axiosClient.get("/categories");
    return response.data;
}

export async function getRootCategories() {
    const response = await axiosClient.get("/categories/root");
    return response.data;
}

export async function getCategoryById(categoryId) {
    const response = await axiosClient.get(`/categories/${categoryId}`);
    return response.data;
}

export async function getCategoryChildren(categoryId) {
    const response = await axiosClient.get(`/categories/${categoryId}/children`);
    return response.data;
}

export async function createCategory(request) {
    const response = await axiosClient.post("/categories", request);
    return response.data;
}

export async function updateCategory(categoryId, request) {
    const response = await axiosClient.put(`/categories/${categoryId}`, request);
    return response.data;
}

export async function deleteCategory(categoryId) {
    await axiosClient.delete(`/categories/${categoryId}`);
}