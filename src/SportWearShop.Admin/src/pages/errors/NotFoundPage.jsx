import { Link } from "react-router-dom";

function NotFoundPage() {
    return (
        <div className="d-flex flex-column justify-content-center align-items-center vh-100 text-center">
            <h1 className="display-1 fw-bold">404</h1>

            <h3 className="fw-semibold">Page Not Found</h3>

            <p className="text-muted mb-4">
                The page you are looking for does not exist.
            </p>

            <Link to="/" className="btn btn-dark">
                Back to Dashboard
            </Link>
        </div>
    );
}

export default NotFoundPage;