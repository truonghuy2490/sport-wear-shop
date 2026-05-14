import { createSlice } from "@reduxjs/toolkit";
import {createUserProfileResponseModel} from "../../models/userModel";
const initialState = {
    currentUser: createUserProfileResponseModel(),
    isAuthenticated: false
};

const authSlice = createSlice({
    name: "auth",
    initialState,
    reducers: {
        login(state, action) {
            state.currentUser = action.payload;
            state.isAuthenticated = true;
        },

        logout(state) {
            state.currentUser = null;
            state.isAuthenticated = false;
        }
    }
});

export const {
    login,
    logout
} = authSlice.actions;

export default authSlice.reducer;