import { Routes, Route } from "react-router-dom";

import Layout from "../layouts/Layout";
import ProtectedRoute from "./ProtectedRoute";

import ProductListPage from "../pages/products/ProductListPage";
import LoginPage from "../pages/auth/LoginPage";

function DashboardPage() {
    return <h2>Dashboard</h2>;
}

function AppRouter() {
    return (
        <Routes>
            <Route
                path="/login"
                element={<LoginPage />}
            />

            <Route
                path="/"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <DashboardPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />

            <Route
                path="/products"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <ProductListPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />
        </Routes>
    );
}

export default AppRouter;