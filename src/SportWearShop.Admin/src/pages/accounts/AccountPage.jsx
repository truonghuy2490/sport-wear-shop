import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

import { getUsers, activateUser, deactivateUser } from "../../api/userApi";

import PageHeader from "../../components/common/PageHeader";
import SearchBox from "../../components/common/SearchBox";
import StatusBadge from "../../components/common/StatusBadge";

function AccountPage() {
    const [users, setUsers] = useState([]);
    const [searchKeyword, setSearchKeyword] = useState("");
    const [isLoading, setIsLoading] = useState(true);

    const [pageNumber, setPageNumber] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(1);

    useEffect(() => {
        loadUsers();
    }, [pageNumber]);

    async function loadUsers() {
        try {
            setIsLoading(true);

            const data = await getUsers(pageNumber, pageSize);

            setUsers(data.items || data.data || []);
            setTotalPages(data.totalPages || 1);
        } catch (error) {
            console.error("Failed to load users:", error);
        } finally {
            setIsLoading(false);
        }
    }

    async function handleToggleStatus(user) {
        try {
            if (user.isActive) {
                await deactivateUser(user.userId);
            } else {
                await activateUser(user.userId);
            }

            await loadUsers();
        } catch (error) {
            console.error("Failed to update user status:", error);
        }
    }

    const filteredUsers = users.filter((user) => {
        const fullName = `${user.firstName || ""} ${user.lastName || ""}`.toLowerCase();

        return (
            user.email.toLowerCase().includes(searchKeyword.toLowerCase()) ||
            fullName.includes(searchKeyword.toLowerCase()) ||
            user.phoneNumber?.toLowerCase().includes(searchKeyword.toLowerCase())
        );
    });

    return (
        <div>
            <PageHeader
                title="Accounts"
                description="Manage customer, staff and admin accounts."
            />

            <div className="card border-0 shadow-sm">
                <div className="card-body">

                    <div className="d-flex justify-content-between align-items-center mb-3">
                        <h5 className="mb-0 fw-semibold">
                            Account List
                        </h5>

                        <SearchBox
                            value={searchKeyword}
                            onChange={setSearchKeyword}
                            placeholder="Search account..."
                        />
                    </div>

                    {isLoading ? (
                        <div className="text-center py-5 text-muted">
                            Loading accounts...
                        </div>
                    ) : (
                        <>
                            <div className="table-responsive">
                                <table className="table table-hover align-middle">
                                    <thead className="table-light">
                                        <tr>
                                            <th>#</th>
                                            <th>User</th>
                                            <th>Email</th>
                                            <th>Phone</th>
                                            <th>Roles</th>
                                            <th>Status</th>
                                            <th>Created At</th>
                                            <th className="text-end">
                                                Actions
                                            </th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        {filteredUsers.length === 0 ? (
                                            <tr>
                                                <td
                                                    colSpan="8"
                                                    className="text-center text-muted py-4"
                                                >
                                                    No accounts found.
                                                </td>
                                            </tr>
                                        ) : (
                                            filteredUsers.map((user, index) => {
                                                const displayName =
                                                    `${user.firstName || ""} ${user.lastName || ""}`.trim() ||
                                                    user.email;

                                                return (
                                                    <tr key={user.userId}>
                                                        <td>
                                                            {(pageNumber - 1) * pageSize + index + 1}
                                                        </td>

                                                        <td className="fw-medium">
                                                            {displayName}
                                                        </td>

                                                        <td>
                                                            {user.email}
                                                        </td>

                                                        <td>
                                                            {user.phoneNumber || "-"}
                                                        </td>

                                                        <td>
                                                            {user.roles?.length > 0
                                                                ? user.roles.join(", ")
                                                                : "-"}
                                                        </td>

                                                        <td>
                                                            <StatusBadge
                                                                isActive={user.isActive}
                                                            />
                                                        </td>

                                                        <td>
                                                            {new Date(user.createdAtUtc).toLocaleDateString()}
                                                        </td>

                                                        <td className="text-end">
                                                            <Link
                                                                to={`/accounts/${user.userId}`}
                                                                className="btn btn-sm btn-outline-primary me-2"
                                                            >
                                                                View
                                                            </Link>

                                                            <button
                                                                type="button"
                                                                className={
                                                                    user.isActive
                                                                        ? "btn btn-sm btn-outline-danger"
                                                                        : "btn btn-sm btn-outline-success"
                                                                }
                                                                onClick={() => handleToggleStatus(user)}
                                                            >
                                                                {user.isActive ? "Deactivate" : "Activate"}
                                                            </button>
                                                        </td>
                                                    </tr>
                                                );
                                            })
                                        )}
                                    </tbody>
                                </table>
                            </div>

                            <div className="d-flex justify-content-between align-items-center mt-3">
                                <div className="text-muted small">
                                    Page {pageNumber} of {totalPages}
                                </div>

                                <div>
                                    <button
                                        type="button"
                                        className="btn btn-sm btn-outline-secondary me-2"
                                        disabled={pageNumber <= 1}
                                        onClick={() => setPageNumber((prev) => prev - 1)}
                                    >
                                        Previous
                                    </button>

                                    <button
                                        type="button"
                                        className="btn btn-sm btn-outline-secondary"
                                        disabled={pageNumber >= totalPages}
                                        onClick={() => setPageNumber((prev) => prev + 1)}
                                    >
                                        Next
                                    </button>
                                </div>
                            </div>
                        </>
                    )}

                </div>
            </div>
        </div>
    );
}

export default AccountPage;