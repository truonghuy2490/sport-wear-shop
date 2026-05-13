export function getUserDisplayName(user) {
    const fullName = `${user.firstName || ""} ${user.lastName || ""}`.trim();

    return fullName || user.email || "Unknown User";
}

export function getUserRolesText(user) {
    if (!user.roles || user.roles.length === 0) {
        return "No role";
    }

    return user.roles.join(", ");
}