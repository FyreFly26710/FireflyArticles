import checkAccess from "@/access/checkAccess";
import menus from "../../config/menus";

/**
 * set unpermitted menu items invisible
 * @param loginUser
 * @param menuItems
 */
const getAccessibleMenus = (loginUser: API.LoginUserResponse, menuItems = menus) => {
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

export default getAccessibleMenus;
