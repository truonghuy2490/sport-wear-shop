import { Routes, Route } from "react-router-dom";

import Layout from "../layouts/Layout";
import ProtectedRoute from "./ProtectedRoute";

import ProductListPage from "../pages/products/ProductListPage";
import LoginPage from "../pages/auth/LoginPage";
import DashboardPage from "../pages/dashboard/DashboardPage";
import BrandPage from "../pages/brands/BrandPage";
import CategoryPage from "../pages/categories/CategoryPage";
import OrderPage from "../pages/orders/OrderPage";
import InventoryPage from "../pages/inventory/InventoryPage";
import AccountPage from "../pages/accounts/AccountPage";



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

            <Route
                path="/categories"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <CategoryPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />

            <Route
                path="/orders"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <OrderPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />

            <Route
                path="/brands"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <BrandPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />
            
            <Route
                path="/inventory"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <InventoryPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />

            <Route
                path="/accounts"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <AccountPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />
        </Routes>
    );
}

export default AppRouter;