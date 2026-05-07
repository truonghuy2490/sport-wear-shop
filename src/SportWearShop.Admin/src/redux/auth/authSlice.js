import { createSlice } from "@reduxjs/toolkit";
import { loginAsync, logoutAsync } from "./authThunks";

const initialState = {
    currentUser: null,
    isAuthenticated: false,
    isLoading: false,
    errorMessage: null
};

const authSlice = createSlice({
    name: "auth",
    initialState,
    reducers: {
        restoreAuthState: (state, action) => {
            state.currentUser = action.payload;
            state.isAuthenticated = true;
        },
        clearAuthState: (state) => {
            state.currentUser = null;
            state.isAuthenticated = false;
            state.errorMessage = null;
        }
    },
    extraReducers: (builder) => {
        builder
            .addCase(loginAsync.pending, (state) => {
                state.isLoading = true;
                state.errorMessage = null;
            })
            .addCase(loginAsync.fulfilled, (state, action) => {
                state.isLoading = false;
                state.isAuthenticated = true;
                state.currentUser = action.payload;
            })
            .addCase(loginAsync.rejected, (state, action) => {
                state.isLoading = false;
                state.isAuthenticated = false;
                state.errorMessage = action.payload;
            })
            .addCase(logoutAsync.fulfilled, (state) => {
                state.currentUser = null;
                state.isAuthenticated = false;
                state.errorMessage = null;
            });
    }
});

export const {
    restoreAuthState,
    clearAuthState
} = authSlice.actions;

export default authSlice.reducer;