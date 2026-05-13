import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";

import {
    getUserDetail,
    activateUser,
    deactivateUser
} from "../../api/userApi";

import StatusBadge from "../../components/common/StatusBadge";

function AccountDetailPage() {
    const { userId } = useParams();

    const [user, setUser] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [isUpdating, setIsUpdating] = useState(false);

    useEffect(() => {
        loadUserDetail();
    }, [userId]);

    async function loadUserDetail() {
        try {
            setIsLoading(true);

            const data = await getUserDetail(userId);

            setUser(data);
        } catch (error) {
            console.error("Failed to load user detail:", error);
        } finally {
            setIsLoading(false);
        }
    }

    async function handleToggleStatus() {
        if (!user) return;

        try {
            setIsUpdating(true);

            if (user.isActive) {
                await deactivateUser(user.userId);
            } else {
                await activateUser(user.userId);
            }

            await loadUserDetail();
        } catch (error) {
            console.error("Failed to update user status:", error);
        } finally {
            setIsUpdating(false);
        }
    }

    if (isLoading) {
        return (
            <div className="text-center py-5 text-muted">
                Loading account detail...
            </div>
        );
    }

    if (!user) {
        return (
            <div className="text-center py-5 text-muted">
                Account not found.
            </div>
        );
    }

    const displayName =
        `${user.firstName || ""} ${user.lastName || ""}`.trim() ||
        user.email;

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">
                        Account Detail
                    </h3>

                    <p className="text-muted mb-0">
                        View user account information, roles and addresses.
                    </p>
                </div>

                <div>
                    <Link
                        to="/accounts"
                        className="btn btn-outline-secondary me-2"
                    >
                        Back
                    </Link>

                    <button
                        type="button"
                        className={
                            user.isActive
                                ? "btn btn-outline-danger"
                                : "btn btn-success"
                        }
                        onClick={handleToggleStatus}
                        disabled={isUpdating}
                    >
                        {isUpdating
                            ? "Updating..."
                            : user.isActive
                                ? "Deactivate"
                                : "Activate"}
                    </button>
                </div>
            </div>

            <div className="card border-0 shadow-sm mb-4">
                <div className="card-body p-4">
                    <div className="d-flex justify-content-between align-items-start mb-3">
                        <div>
                            <h4 className="fw-bold mb-1">
                                {displayName}
                            </h4>

                            <div className="text-muted">
                                {user.email}
                            </div>
                        </div>

                        <StatusBadge isActive={user.isActive} />
                    </div>

                    <hr />

                    <div className="row g-4">
                        <div className="col-md-6">
                            <div className="text-muted small">
                                User ID
                            </div>
                            <div className="fw-medium">
                                {user.userId}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Username
                            </div>
                            <div className="fw-medium">
                                {user.userName || "-"}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Phone Number
                            </div>
                            <div className="fw-medium">
                                {user.phoneNumber || "-"}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Roles
                            </div>
                            <div className="fw-medium">
                                {user.roles?.length > 0
                                    ? user.roles.join(", ")
                                    : "-"}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Email Confirmed
                            </div>
                            <div className="fw-medium">
                                {user.emailConfirmed ? "Yes" : "No"}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Phone Confirmed
                            </div>
                            <div className="fw-medium">
                                {user.phoneNumberConfirmed ? "Yes" : "No"}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Created At
                            </div>
                            <div className="fw-medium">
                                {new Date(user.createdAtUtc).toLocaleString()}
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="text-muted small">
                                Updated At
                            </div>
                            <div className="fw-medium">
                                {new Date(user.updatedAtUtc).toLocaleString()}
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div className="card border-0 shadow-sm">
                <div className="card-body">
                    <h5 className="fw-semibold mb-3">
                        Addresses
                    </h5>

                    {user.addresses?.length === 0 ? (
                        <div className="text-muted">
                            No addresses found.
                        </div>
                    ) : (
                        <div className="table-responsive">
                            <table className="table table-hover align-middle">
                                <thead className="table-light">
                                    <tr>
                                        <th>#</th>
                                        <th>Recipient</th>
                                        <th>Phone</th>
                                        <th>Address</th>
                                        <th>City</th>
                                        <th>Default</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    {user.addresses.map((address, index) => (
                                        <tr key={address.userAddressId}>
                                            <td>{index + 1}</td>

                                            <td className="fw-medium">
                                                {address.recipientName}
                                            </td>

                                            <td>
                                                {address.phoneNumber}
                                            </td>

                                            <td>
                                                {address.addressLine1}
                                                {address.addressLine2
                                                    ? `, ${address.addressLine2}`
                                                    : ""}
                                                {address.ward
                                                    ? `, ${address.ward}`
                                                    : ""}
                                                {address.district
                                                    ? `, ${address.district}`
                                                    : ""}
                                            </td>

                                            <td>
                                                {address.city}
                                            </td>

                                            <td>
                                                {address.isDefaultShipping && (
                                                    <span className="badge bg-dark me-1">
                                                        Shipping
                                                    </span>
                                                )}

                                                {address.isDefaultBilling && (
                                                    <span className="badge bg-secondary">
                                                        Billing
                                                    </span>
                                                )}

                                                {!address.isDefaultShipping &&
                                                    !address.isDefaultBilling &&
                                                    "-"}
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default AccountDetailPage;