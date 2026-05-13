export const userAddressResponseModel = {
    userAddressId: 0,
    recipientName: "",
    phoneNumber: "",
    addressLine1: "",
    addressLine2: "",
    ward: "",
    district: "",
    city: "",
    province: "",
    postalCode: "",
    countryCode: "",
    isDefaultShipping: false,
    isDefaultBilling: false
};

export const userResponseModel = {
    userId: 0,
    email: "",
    firstName: "",
    lastName: "",
    phoneNumber: "",
    isActive: true,
    roles: [],
    createdAtUtc: ""
};

export const userDetailResponseModel = {
    userId: 0,
    email: "",
    userName: "",
    firstName: "",
    lastName: "",
    phoneNumber: "",
    emailConfirmed: false,
    phoneNumberConfirmed: false,
    isActive: true,
    roles: [],
    createdAtUtc: "",
    updatedAtUtc: "",
    addresses: []
};

export const userProfileResponseModel = {
    userId: 0,
    email: "",
    firstName: "",
    lastName: "",
    phoneNumber: "",
    isActive: true,
    createdAtUtc: ""
};

export const createUserProfileResponseModel = (data = {}) => ({
    userId: data.userId ?? 0,
    email: data.email ?? "",
    firstName: data.firstName ?? "",
    lastName: data.lastName ?? "",
    phoneNumber: data.phoneNumber ?? "",
    isActive: data.isActive ?? true,
    createdAtUtc: data.createdAtUtc ?? "",

    displayName:
        data.firstName && data.lastName
            ? `${data.firstName} ${data.lastName}`
            : data.email ?? ""
});