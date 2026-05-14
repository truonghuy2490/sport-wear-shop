import { NavLink } from "react-router-dom";

function Sidebar() {
    const menuItems = [
        {
            path: "/",
            label: "Dashboard",
            icon: "bi-speedometer2"
        },
        {
            path: "/products",
            label: "Products",
            icon: "bi-box-seam"
        },
        {
            path: "/categories",
            label: "Categories",
            icon: "bi-tags"
        },
        {
            path: "/brands",
            label: "Brands",
            icon: "bi-patch-check"
        },
        // {
        //     path: "/inventory",
        //     label: "Inventory",
        //     icon: "bi-archive"
        // },
        {
            path: "/orders",
            label: "Orders",
            icon: "bi-receipt"
        },
        {
            path: "/accounts",
            label: "Accounts",
            icon: "bi-people"
        }
    ];

    return (
        <>
            <aside
                className="bg-white border-end d-none d-lg-block"
                style={{
                    width: "260px",
                    minHeight: "calc(100vh - 73px)"
                }}
            >
                <div className="p-4">
                    <p className="text-muted small fw-semibold mb-3">
                        MANAGEMENT
                    </p>

                    <nav className="nav flex-column gap-1">
                        {menuItems.map((item) => (
                            <NavLink
                                key={item.path}
                                to={item.path}
                                className={({ isActive }) =>
                                    `nav-link rounded-3 px-3 py-2 fw-medium ${
                                        isActive
                                            ? "bg-dark text-white"
                                            : "text-dark"
                                    }`
                                }
                            >
                                <i className={`bi ${item.icon} me-2`}></i>
                                {item.label}
                            </NavLink>
                        ))}
                    </nav>
                </div>
            </aside>

            <div
                className="offcanvas offcanvas-start"
                tabIndex="-1"
                id="adminSidebar"
            >
                <div className="offcanvas-header">
                    <h5 className="fw-bold mb-0">SportWear Admin</h5>

                    <button
                        type="button"
                        className="btn-close"
                        data-bs-dismiss="offcanvas"
                    ></button>
                </div>

                <div className="offcanvas-body">
                    <nav className="nav flex-column gap-1">
                        {menuItems.map((item) => (
                            <NavLink
                                key={item.path}
                                to={item.path}
                                className="nav-link text-dark rounded-3 px-3 py-2 fw-medium"
                            >
                                <i className={`bi ${item.icon} me-2`}></i>
                                {item.label}
                            </NavLink>
                        ))}
                    </nav>
                </div>
            </div>
        </>
    );
}

export default Sidebar;