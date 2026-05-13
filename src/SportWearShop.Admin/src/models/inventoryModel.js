export const inventoryStockResponseModel = {
    productVariantId: 0,
    sku: "",
    quantityOnHand: 0,
    quantityReserved: 0,
    availableStock: 0,
    updatedAtUtc: ""
};

export const inventoryMovementResponseModel = {
    inventoryMovementId: 0,
    productVariantId: 0,
    sku: "",
    movementType: "",
    quantity: 0,
    referenceType: "",
    referenceId: null,
    note: "",
    createdAtUtc: ""
};

export const stockInRequestModel = {
    productVariantId: 0,
    quantity: 0,
    staffId: 0,
    note: ""
};

export const stockOutRequestModel = {
    productVariantId: 0,
    quantity: 0,
    staffId: 0,
    note: ""
};

export const reserveStockRequestModel = {
    productVariantId: 0,
    quantity: 0,
    orderId: 0,
    note: ""
};

export const releaseStockRequestModel = {
    productVariantId: 0,
    quantity: 0,
    orderId: 0,
    note: ""
};

export const soldStockRequestModel = {
    productVariantId: 0,
    quantity: 0,
    orderId: 0,
    note: ""
};