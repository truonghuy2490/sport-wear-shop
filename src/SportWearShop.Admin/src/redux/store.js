import { configureStore } from "@reduxjs/toolkit";

const initialAuthState = {
    currentUser: null,
    isAuthenticated: false,
    isLoading: false,
    errorMessage: null
};

function authReducer(state = initialAuthState, action) {
    switch (action.type) {
        default:
            return state;
    }
}

export const store = configureStore({
    reducer: {
        auth: authReducer
    }
});