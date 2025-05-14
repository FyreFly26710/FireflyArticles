import { MenuDataItem } from "@ant-design/pro-layout";
import { CrownOutlined } from "@ant-design/icons";
import ACCESS_ENUM from "@/libs/constants/accessEnum";

const menus = [
    {
        path: "/",
        name: "Home",
    },
    {
        path: "/topics",
        name: "Topics",
    },
    {
        path: "/articles",
        name: "Articles",
    },
    {
        path: "/aichat",
        name: "AI Chat",
    },
    {
        path: "/aigen",
        name: "AI Generate",
    },
    {
        path: "/admin",
        name: "Admin",
        icon: <CrownOutlined />,
        access: ACCESS_ENUM.ADMIN,
        children: [
            {
                path: "/admin/user",
                name: "User Management",
                access: ACCESS_ENUM.ADMIN,
            },
            {
                path: "/admin/topic",
                name: "Topic Management",
                access: ACCESS_ENUM.ADMIN,
            },
            {
                path: "/admin/article",
                name: "Article Management",
                access: ACCESS_ENUM.ADMIN,
            },
            {
                path: "/admin/tag",
                name: "Tag Management",
                access: ACCESS_ENUM.ADMIN,
            },
        ],
    }

] as MenuDataItem[];

export default menus;

// find All Menu Item By Path
export const findAllMenuItemByPath = (path: string): MenuDataItem | null => {
    return findMenuItemByPath(menus, path);
};

// find Menu Item By Path
export const findMenuItemByPath = (
    menus: MenuDataItem[],
    path: string,
): MenuDataItem | null => {
    for (const menu of menus) {
        if (menu.path === path) {
            return menu;
        }
        if (menu.children) {
            const matchedMenuItem = findMenuItemByPath(menu.children, path);
            if (matchedMenuItem) {
                return matchedMenuItem;
            }
        }
    }
    return null;
};
