import { Navigate } from "react-router-dom";
import { useSelector } from "react-redux";

import { getAccessToken } from "../utils/tokenStorage";

function ProtectedRoute({ children }) {
    const isAuthenticated = useSelector(
        (state) => state.auth.isAuthenticated
    );

    const token = getAccessToken();

    if (!isAuthenticated && !token) {
        return <Navigate to="/login" replace />;
    }

    return children;
}

export default ProtectedRoute;