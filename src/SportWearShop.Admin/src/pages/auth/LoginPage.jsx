import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { login } from "../../api/authApi";
import { saveTokens } from "../../utils/tokenStorage";

function LoginPage() {

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [errorMessage, setErrorMessage] = useState("");

    const navigate = useNavigate();

    const handleSubmit = async (e) => {

        e.preventDefault();

        setErrorMessage("");

        try {
            
            const result = await login({
                email,
                password
            });

            saveTokens(
                result.accessToken,
                result.refreshToken
            );

            navigate("/");

        } catch (error) {

            setErrorMessage(
                error.response?.data?.message ||
                "Login failed. Please check your email and password."
            );
        }
    };

    return (
        <div
            className="d-flex align-items-center justify-content-center py-5 bg-light"
            style={{ minHeight: "100vh" }}
        >
            <div className="col-12 col-sm-10 col-md-7 col-lg-5">

                <div
                    className="card border-0 shadow-sm rounded-4"
                    style={{ borderRadius: "24px" }}
                >
                    <div className="card-body p-4 p-lg-5">

                        <div className="text-center mb-4">

                            <h1 className="fw-bold mb-2">
                                Admin Login
                            </h1>

                            <p className="text-muted mb-0">
                                Login to manage Sport Wear Shop.
                            </p>

                        </div>

                        {errorMessage && (
                            <div className="alert alert-danger py-2 small mb-4">
                                {errorMessage}
                            </div>
                        )}

                        <form
                            onSubmit={handleSubmit}
                            style={{
                                borderRadius: "14px",
                                padding: "0.85rem 1rem"
                            }}
                        >

                            <div className="mb-3">

                                <label className="form-label fw-medium">
                                    Email
                                </label>

                                <input
                                    type="email"
                                    className="form-control form-control-lg"
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                    placeholder="admin@example.com"
                                />

                            </div>

                            <div className="mb-4">

                                <label className="form-label fw-medium">
                                    Password
                                </label>

                                <input
                                    type="password"
                                    className="form-control form-control-lg"
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    placeholder="Enter password"
                                />

                            </div>

                            <button
                                type="submit"
                                className="btn btn-dark btn-lg w-100"
                                style={{ borderRadius: "14px" }}
                            >
                                Login
                            </button>

                        </form>

                    </div>
                </div>

            </div>
        </div>
    );
}

export default LoginPage;