import axiosClient from "./axiosClient";
import { clearTokens } from "../utils/tokenStorage";

export async function login(request) {
    const response = await axiosClient.post("/auth/login", request);
    return response.data;
}

export async function register(request) {
    const response = await axiosClient.post("/auth/register", request);
    return response.data;
}

export async function refreshToken(request) {
    const response = await axiosClient.post("/auth/refresh-token", request);
    return response.data;
}

export async function logout() {
    await axiosClient.post("/auth/logout");
    clearTokens();
}