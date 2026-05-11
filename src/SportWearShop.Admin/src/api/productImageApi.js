// src/api/productImageApi.js

import axiosClient from "./axiosClient";

function buildProductImageFormData(request) {
    const formData = new FormData();

    formData.append("productId", request.productId);

    if (request.productVariantId) {
        formData.append("productVariantId", request.productVariantId);
    }

    formData.append("imageUrl", request.imageUrl);
    formData.append("altText", request.altText || "");
    formData.append("sortOrder", request.sortOrder);
    formData.append("isPrimary", request.isPrimary);

    return formData;
}

export async function getProductImagesByProductId(productId) {
    const response = await axiosClient.get(
        `/api/product/${productId}/product-images`
    );

    return response.data;
}

export async function getProductImagesByVariantId(productVariantId) {
    const response = await axiosClient.get(
        `/api/variant/${productVariantId}/product-images`
    );

    return response.data;
}

export async function createProductImage(request) {
    const formData = buildProductImageFormData(request);

    const response = await axiosClient.post(
        "/api/product-images",
        formData,
        {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        }
    );

    return response.data;
}

export async function updateProductImage(productImageId, request) {
    const formData = buildProductImageFormData(request);

    const response = await axiosClient.put(
        `/api/product-images/${productImageId}`,
        formData,
        {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        }
    );

    return response.data;
}

export async function setPrimaryProductImage(productImageId) {
    await axiosClient.patch(
        `/api/product-images/${productImageId}/set-primary`
    );
}

export async function deleteProductImage(productImageId) {
    await axiosClient.delete(`/api/product-images/${productImageId}`);
}