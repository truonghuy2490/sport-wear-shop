import axiosClient from "./axiosClient";

export async function getStockByVariantId(productVariantId) {
    const response = await axiosClient.get(
        `/inventory/${productVariantId}/stock`
    );

    return response.data;
}

export async function getMovementsByVariantId(productVariantId) {
    const response = await axiosClient.get(
        `/inventory/${productVariantId}/movements`
    );

    return response.data;
}

export async function stockIn(request) {
    const response = await axiosClient.post(
        "/inventory/stock-in",
        request
    );

    return response.data;
}

export async function stockOut(request) {
    const response = await axiosClient.post(
        "/inventory/stock-out",
        request
    );

    return response.data;
}