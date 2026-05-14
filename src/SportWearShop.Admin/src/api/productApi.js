import axiosClient from "./axiosClient";

export async function getProducts(query) {
    const params = {
        pageNumber: query.pageNumber,
        pageSize: query.pageSize,
        sortBy: query.sortBy,
        isAscending: query.isAscending
    };

    if (query.searchTerm?.trim()) {
        params.searchTerm = query.searchTerm.trim();
    }

    if (query.status !== "") {
        params.status = Number(query.status);
    }

    const response = await axiosClient.get("/products", {
        params
    });

    return response.data;
}

export async function getProductDetail(productId) {
    const response = await axiosClient.get(`/admin/products/${productId}`);
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

export async function updateProductStatus(productId, status) {
    const response = await axiosClient.patch(
        `/products/${productId}/status`,
        {
            status: Number(status)
        }
    );

    return response.data;
}