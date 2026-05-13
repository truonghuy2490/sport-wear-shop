// src/api/productVariantApi.js

import axiosClient from "./axiosClient";

export async function getProductVariantsByProductId(productId) {
    const response = await axiosClient.get(
        `/products/${productId}/product-variants`
    );

    return response.data;
}

export async function getProductVariantById(productVariantId) {
    const response = await axiosClient.get(
        `/product-variants/${productVariantId}`
    );

    return response.data;
}

export async function createProductVariants(productId, request) {
    const response = await axiosClient.post(
        `/products/${productId}/product-variants/batch`,
        request
    );

    return response.data;
}

export async function updateProductVariant(productVariantId, request) {
    const response = await axiosClient.put(
        `/product-variants/${productVariantId}`,
        request
    );

    return response.data;
}

export async function deleteProductVariant(productVariantId) {
    await axiosClient.delete(
        `/product-variants/${productVariantId}`
    );
}

export async function updateProductVariantImageSortOrders(
    productVariantId,
    request
) {
    const response = await axiosClient.put(
        `/product-variants/${productVariantId}/images/sort-orders`,
        request
    );

    return response.data;
}