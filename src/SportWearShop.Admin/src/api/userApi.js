import axiosClient from "./axiosClient";
import {createUserProfileResponseModel} from "../models/userModel";

export async function getUsers(pageNumber = 1, pageSize = 10) {
    const response = await axiosClient.get("/users", {
        params: {
            pageNumber,
            pageSize
        }
    });

    return response.data;
}

export async function getUserDetail(userId) {
    const response = await axiosClient.get(`/users/${userId}`);
    return response.data;
}

export async function activateUser(userId) {
    const response = await axiosClient.patch(`/users/${userId}/activate`);
    return response.data;
}

export async function deactivateUser(userId) {
    const response = await axiosClient.patch(`/users/${userId}/deactivate`);
    return response.data;
}


export const getCurrentUser = async () => {
    const response = await axiosClient.get("/users/me");
    return createUserProfileResponseModel(response.data);
};