import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";

import { logout as logoutApi } from "../api/authApi";
import { logout as logoutAction } from "../redux/auth/authSlice";
import { clearTokens } from "../utils/tokenStorage";

function useLogout() {
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const handleLogout = async () => {
        try {
            await logoutApi();
        } catch (error) {
            console.error("Logout request failed:", error);
        } finally {
            clearTokens();

            dispatch(logoutAction());
            
            navigate("/login", {
                replace: true
            });
        }
    };

    return handleLogout;
}

export default useLogout;