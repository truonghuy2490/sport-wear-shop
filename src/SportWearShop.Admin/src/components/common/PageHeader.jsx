function PageHeader({
    title,
    description,
    actionText,
    onActionClick
}) {
    return (
        <div className="d-flex justify-content-between align-items-center mb-4">
            <div>
                <h3 className="fw-bold mb-1">{title}</h3>
                <p className="text-muted mb-0">{description}</p>
            </div>

            {actionText && (
                <button
                    className="btn btn-dark"
                    onClick={onActionClick}
                >
                    {actionText}
                </button>
            )}
        </div>
    );
}

export default PageHeader;