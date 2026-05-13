import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { hideToast } from "../../redux/toast/toastSlice";

function ToastContainer() {
    const dispatch = useDispatch();

    const {
        isVisible,
        type,
        title,
        message
    } = useSelector((state) => state.toast);

    useEffect(() => {
        if (!isVisible) return;

        const timer = setTimeout(() => {
            dispatch(hideToast());
        }, type === "error" ? 4000 : 3000);

        return () => clearTimeout(timer);
    }, [isVisible, type, dispatch]);

    if (!isVisible) return null;

    return (
        <div
            className="toast-container position-fixed top-0 end-0 p-3 mt-5"
            style={{ zIndex: 1080 }}
        >
            <div
                className="sport-toast border-0 shadow-sm mb-2 bg-white"
                role="alert"
            >
                <div className="d-flex align-items-center">

                    <div
                        className={`toast-indicator ${
                            type === "success"
                                ? "bg-success"
                                : "bg-danger"
                        }`}
                    />

                    <div className="toast-body">
                        <strong>{title}</strong>

                        <div className="text-muted small">
                            {message}
                        </div>
                    </div>

                    <button
                        type="button"
                        className="btn-close me-3 m-auto"
                        onClick={() => dispatch(hideToast())}
                    />
                </div>
            </div>
        </div>
    );
}

export default ToastContainer;