import { configureStore } from "@reduxjs/toolkit";
import authReducer from "./auth/authSlice";
import toastReducer from "./toast/toastSlice";

export const store = configureStore({
    reducer: {
        auth: authReducer,
        toast: toastReducer
    }
});