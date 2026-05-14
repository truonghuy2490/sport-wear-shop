import { Link } from "react-router-dom";

function DashboardPage() {
    return (
        <div className="container-fluid py-4">
            <div className="mb-4">
                <h2 className="fw-bold mb-1">Admin Dashboard</h2>
                <p className="text-muted">
                    Welcome to SportWearShop management system.
                </p>
            </div>

            <div className="row g-4">
                {/* SYSTEM STATUS */}
                <div className="col-12 col-lg-8">
                    <div className="card border-0 shadow-sm">
                        <div className="card-body">
                            <h5 className="fw-bold mb-4">System Modules</h5>

                            <div className="row g-3">
                                <div className="col-md-6">
                                    <div className="border rounded p-3">
                                        <div className="d-flex justify-content-between align-items-center">
                                            <div>
                                                <h6 className="fw-bold mb-1">Product Management</h6>
                                                <small className="text-muted">
                                                    Catalog, variants, pricing, images
                                                </small>
                                            </div>
                                            <span className="badge bg-success">Active</span>
                                        </div>
                                    </div>
                                </div>

                                <div className="col-md-6">
                                    <div className="border rounded p-3">
                                        <div className="d-flex justify-content-between align-items-center">
                                            <div>
                                                <h6 className="fw-bold mb-1">Category Management</h6>
                                                <small className="text-muted">
                                                    Tree structure categories
                                                </small>
                                            </div>
                                            <span className="badge bg-success">Active</span>
                                        </div>
                                    </div>
                                </div>

                                <div className="col-md-6">
                                    <div className="border rounded p-3">
                                        <div className="d-flex justify-content-between align-items-center">
                                            <div>
                                                <h6 className="fw-bold mb-1">Inventory</h6>
                                                <small className="text-muted">
                                                    Stock + inventory movement
                                                </small>
                                            </div>
                                            <span className="badge bg-success">Active</span>
                                        </div>
                                    </div>
                                </div>

                                <div className="col-md-6">
                                    <div className="border rounded p-3">
                                        <div className="d-flex justify-content-between align-items-center">
                                            <div>
                                                <h6 className="fw-bold mb-1">Order Processing</h6>
                                                <small className="text-muted">
                                                    Checkout + payment flow
                                                </small>
                                            </div>
                                            <span className="badge bg-warning text-dark">
                                                In Progress
                                            </span>
                                        </div>
                                    </div>
                                </div>

                                <div className="col-md-6">
                                    <div className="border rounded p-3">
                                        <div className="d-flex justify-content-between align-items-center">
                                            <div>
                                                <h6 className="fw-bold mb-1">Authentication</h6>
                                                <small className="text-muted">
                                                    JWT + refresh token
                                                </small>
                                            </div>
                                            <span className="badge bg-success">Active</span>
                                        </div>
                                    </div>
                                </div>

                                <div className="col-md-6">
                                    <div className="border rounded p-3">
                                        <div className="d-flex justify-content-between align-items-center">
                                            <div>
                                                <h6 className="fw-bold mb-1">Customer Storefront</h6>
                                                <small className="text-muted">
                                                    Razor Pages ecommerce UI
                                                </small>
                                            </div>
                                            <span className="badge bg-primary">
                                                Ongoing
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                {/* QUICK ACTIONS */}
                <div className="col-12 col-lg-4">
                    <div className="card border-0 shadow-sm h-100">
                        <div className="card-body">
                            <h5 className="fw-bold mb-4">Quick Actions</h5>

                            <div className="d-grid gap-2">
                                <Link to="/products/create" className="btn btn-dark">
                                    Add New Product
                                </Link>

                                <Link to="/products" className="btn btn-outline-dark">
                                    Manage Products
                                </Link>

                                {/* <Link to="/inventory" className="btn btn-outline-dark">
                                    Inventory
                                </Link> */}

                                <Link to="/categories" className="btn btn-outline-dark">
                                    Categories
                                </Link>

                                <Link to="/orders" className="btn btn-outline-dark">
                                    Orders
                                </Link>
                            </div>
                        </div>
                    </div>
                </div>

                {/* DEVELOPMENT NOTES */}
                <div className="col-12">
                    <div className="card border-0 shadow-sm">
                        <div className="card-body">
                            <h5 className="fw-bold mb-3">Project Architecture</h5>

                            <div className="row">
                                <div className="col-md-3">
                                    <strong>Frontend Admin</strong>
                                    <p className="text-muted small mb-0">
                                        ReactJS
                                    </p>
                                </div>

                                <div className="col-md-3">
                                    <strong>Customer Web</strong>
                                    <p className="text-muted small mb-0">
                                        ASP.NET Razor Pages
                                    </p>
                                </div>

                                <div className="col-md-3">
                                    <strong>Backend APIs</strong>
                                    <p className="text-muted small mb-0">
                                        ASP.NET Core 3-layer architecture
                                    </p>
                                </div>

                                <div className="col-md-3">
                                    <strong>Shared Models</strong>
                                    <p className="text-muted small mb-0">
                                        Shared ViewModels project
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default DashboardPage;