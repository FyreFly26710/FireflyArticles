import ACCESS_ENUM from "@/libs/constants/accessEnum";
import menus from "../../../config/menus";


export const checkAccess = (loginUser: API.LoginUserDto, needAccess = ACCESS_ENUM.NOT_LOGIN) => {
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

export const getAccessibleMenus = (loginUser: API.LoginUserDto, menuItems = menus) => {
    return menuItems.filter((item) => {
        if (!checkAccess(loginUser, item.access)) {
            return false;
        }
        if (item.children) {
            item.children = getAccessibleMenus(loginUser, item.children);
        }
        return true;
    });
};

