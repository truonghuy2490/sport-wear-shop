import { useSelector } from "react-redux";
import useLogout from "../hooks/useLogout";
function Header() {

    const currentUser = useSelector(
        (state) => state.auth.currentUser
    );
    const handleLogout = useLogout();
    return (
        <header className="bg-white border-bottom sticky-top">
            <nav className="navbar navbar-expand-lg px-4 py-3">

                <button
                    className="btn btn-light d-lg-none me-3"
                    type="button"
                    data-bs-toggle="offcanvas"
                    data-bs-target="#adminSidebar"
                >
                    <i className="bi bi-list"></i>
                </button>

                <a className="navbar-brand fw-bold fs-3" href="/">
                    SportWearShop
                    <span className="text-muted fs-6 ms-2">
                        Admin
                    </span>
                </a>

                <div className="ms-auto d-flex align-items-center gap-3">

                    {/* <form className="position-relative d-none d-md-block">
                        <i className="bi bi-search position-absolute top-50 start-0 translate-middle-y ms-3 text-muted"></i>

                        <input
                            type="text"
                            className="form-control ps-5"
                            placeholder="Search admin..."
                            style={{ minWidth: "260px" }}
                        />
                    </form> */}

                    <div className="dropdown">
                        <button
                            className="btn btn-light dropdown-toggle fw-medium"
                            data-bs-toggle="dropdown"
                        >
                            Hello, {currentUser?.displayName || currentUser?.email || "Admin"}
                        </button>

                        <ul className="dropdown-menu dropdown-menu-end">

                            <li>
                                <a
                                    className="dropdown-item"
                                    href="/profile"
                                >
                                    Profile
                                </a>
                            </li>

                            <li>
                                <hr className="dropdown-divider" />
                            </li>

                            <li>
                                <button
                                    type="button"
                                    className="dropdown-item text-danger"
                                    onClick={handleLogout}
                                >
                                    Logout
                                </button>
                            </li>

                        </ul>
                    </div>

                </div>
            </nav>
        </header>
    );
}

export default Header;