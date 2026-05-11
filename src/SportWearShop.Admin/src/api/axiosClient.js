import axios from "axios";
import {
    getAccessToken,
    getRefreshToken,
    getUserId,
    saveTokens,
    clearTokens
} from "../utils/tokenStorage";

const axiosClient = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    headers: {
        "Content-Type": "application/json"
    }
});

// add accesstoken to each request
axiosClient.interceptors.request.use(
    (config) => {
        const token = getAccessToken();

        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }

        return config;
    },
    (error) => Promise.reject(error)
);

// auto refresh token when exprired or 401 return: reactive refresh 
axiosClient.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        if (!originalRequest) {
            return Promise.reject(error);
        }

        const isAuthRequest =
            originalRequest.url?.includes("/auth/login") ||
            originalRequest.url?.includes("/auth/register") ||
            originalRequest.url?.includes("/auth/refresh-token") ||
            originalRequest.url?.includes("/auth/logout");

        if (
            error.response?.status === 401 &&
            !originalRequest._retry &&
            !isAuthRequest
        ) {
            originalRequest._retry = true;

            try {
                const userId = getUserId();
                const refreshToken = getRefreshToken();

                if (!userId || !refreshToken) {
                    clearTokens();
                    window.location.href = "/login";
                    return Promise.reject(error);
                }

                const response = await axios.post(
                    `${import.meta.env.VITE_API_BASE_URL}/auth/refresh-token`,
                    {
                        userId: Number(userId),
                        refreshToken: refreshToken
                    }
                );

                const { accessToken, refreshToken: newRefreshToken } = response.data;

                saveTokens(
                    accessToken,
                    newRefreshToken,
                    userId
                );

                originalRequest.headers = originalRequest.headers ?? {};
                originalRequest.headers.Authorization = `Bearer ${accessToken}`;

                return axiosClient(originalRequest);
            } catch (refreshError) {
                clearTokens();
                window.location.href = "/login";
                return Promise.reject(refreshError);
            }
        }

        return Promise.reject(error);
    }
);

export default axiosClient;