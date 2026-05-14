function StepIndicator({ currentStep }) {
    const steps = [
        { number: 1, label: "Product" },
        { number: 2, label: "Variants" },
        { number: 3, label: "Images" }
    ];

    return (
        <div className="card border-0 shadow-sm mb-4">
            <div className="card-body">
                <div className="d-flex justify-content-between">
                    {steps.map((step) => (
                        <div
                            key={step.number}
                            className="d-flex align-items-center gap-2"
                        >
                            <span
                                className={
                                    currentStep >= step.number
                                        ? "badge rounded-pill bg-dark"
                                        : "badge rounded-pill bg-secondary"
                                }
                            >
                                {step.number}
                            </span>

                            <span
                                className={
                                    currentStep === step.number
                                        ? "fw-semibold"
                                        : "text-muted"
                                }
                            >
                                {step.label}
                            </span>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}

export default StepIndicator;