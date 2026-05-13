import { createSlice } from "@reduxjs/toolkit";

const initialState = {
    isVisible: false,
    type: "success",
    title: "",
    message: ""
};

const toastSlice = createSlice({
    name: "toast",
    initialState,
    reducers: {
        showToast(state, action) {
            state.isVisible = true;
            state.type = action.payload.type || "success";
            state.title = action.payload.title || "";
            state.message = action.payload.message || "";
        },

        hideToast(state) {
            state.isVisible = false;
            state.title = "";
            state.message = "";
        }
    }
});

export const {
    showToast,
    hideToast
} = toastSlice.actions;

export default toastSlice.reducer;