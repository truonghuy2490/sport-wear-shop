function StatusBadge({ isActive }) {
    return (
        <span
            className={
                isActive
                    ? "badge bg-success"
                    : "badge bg-secondary"
            }
        >
            {isActive ? "Active" : "Inactive"}
        </span>
    );
}

export default StatusBadge;