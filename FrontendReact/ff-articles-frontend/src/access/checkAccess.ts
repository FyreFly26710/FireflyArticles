import ACCESS_ENUM from "@/access/accessEnum";

/**
 * Check permissions (determine if the current logged-in user has a specific permission)
 * @param loginUser The currently logged-in user
 * @param needAccess The required permission
 * @return boolean Whether the user has the permission
 */
const checkAccess = (loginUser: API.LoginUserResponse, needAccess = ACCESS_ENUM.NOT_LOGIN) => {
    const loginUserAccess = loginUser?.userRole ?? ACCESS_ENUM.NOT_LOGIN;
    if (needAccess === ACCESS_ENUM.NOT_LOGIN) {
        return true;
    }
    if (needAccess === ACCESS_ENUM.USER) {
        if (loginUserAccess === ACCESS_ENUM.NOT_LOGIN) {
            return false;
        }
    }
    if (needAccess === ACCESS_ENUM.ADMIN) {
        if (loginUserAccess !== ACCESS_ENUM.ADMIN) {
            return false;
        }
    }
    return true;
};

export default checkAccess;
