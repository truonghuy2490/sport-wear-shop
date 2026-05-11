import { Routes, Route } from "react-router-dom";

import Layout from "../layouts/Layout";
import ProtectedRoute from "./ProtectedRoute";

import ProductListPage from "../pages/products/ProductListPage";
import ProductDetailPage from "../pages/products/ProductDetailPage";
import ProductCreatePage from "../pages/products/ProductCreatePage";

import LoginPage from "../pages/auth/LoginPage";
import DashboardPage from "../pages/dashboard/DashboardPage";

import BrandPage from "../pages/brands/BrandPage";
import BrandDetailPage from "../pages/brands/BrandDetailPage";
import BrandEditPage from "../pages/brands/BrandEditPage";

import CategoryPage from "../pages/categories/CategoryPage";
import CategoryDetailPage from "../pages/categories/CategoryDetailPage";
import CategoryCreatePage from "../pages/categories/CategoryCreatePage";
import CategoryEditPage from "../pages/categories/CategoryEditPage";

import OrderPage from "../pages/orders/OrderPage";
import InventoryPage from "../pages/inventory/InventoryPage";
import AccountPage from "../pages/accounts/AccountPage";
import AccountDetailPage from "../pages/accounts/AccountDetailPage";



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

            {/* PRODUCT */}
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
                path="/products/:productId"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <ProductDetailPage/>
                        </Layout>
                    </ProtectedRoute>
                }
            />
            <Route
                path="/products/create"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <ProductCreatePage/>
                        </Layout>
                    </ProtectedRoute>
                }
            />

            {/* CATEGORY */}
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
                path="/categories/create"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <CategoryCreatePage />
                        </Layout>
                    </ProtectedRoute>
                }
            />

            <Route
                path="/categories/:categoryId"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <CategoryDetailPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />

            <Route
                path="/categories/:categoryId/edit"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <CategoryEditPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />

            {/* ORDER */}
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
            {/* BRAND */}
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
                path="/brands/:brandId"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <BrandDetailPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />

            <Route
                path="/brands/:brandId/edit"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <BrandEditPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />

            {/* Inventory */}
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
            {/* ACCOUNT */}
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
            <Route
                path="/accounts/:userId"
                element={
                    <ProtectedRoute>
                        <Layout>
                            <AccountDetailPage />
                        </Layout>
                    </ProtectedRoute>
                }
            />
        </Routes>
    );
}

export default AppRouter;