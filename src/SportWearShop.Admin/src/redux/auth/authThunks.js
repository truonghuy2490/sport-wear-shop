import { createAsyncThunk } from "@reduxjs/toolkit";
import { login, logout } from "../../api/authApi";
import { saveTokens, clearTokens } from "../../utils/tokenStorage";

export const loginAsync = createAsyncThunk(
    "auth/login",
    async (request, thunkAPI) => {
        try {
            const result = await login(request);

            saveTokens(
                result.accessToken,
                result.refreshToken
            );

            return {
                email: request.email,
                role: "Admin"
            };
        } catch (error) {
            return thunkAPI.rejectWithValue(
                error.response?.data?.message ||
                "Login failed. Please try again."
            );
        }
    }
);

export const logoutAsync = createAsyncThunk(
    "auth/logout",
    async () => {
        try {
            await logout();
        } finally {
            clearTokens();
        }
    }
);