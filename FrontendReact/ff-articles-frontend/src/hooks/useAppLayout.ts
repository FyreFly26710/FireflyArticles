import { useState, useEffect } from 'react';
import { usePathname, useRouter } from "next/navigation";
import { useSelector } from "react-redux";
import { RootState } from "@/stores";
import { MenuProps } from 'antd';
import { MenuDataItem } from "@ant-design/pro-components";
import { getAccessibleMenus } from '@/libs/utils/accessUtils';
import menus from '../../config/menus';

type MenuItem = Required<MenuProps>['items'][number];

export const useAppLayout = () => {
    const pathname = usePathname();
    const router = useRouter();
    const loginUser = useSelector((state: RootState) => state.loginUser);
    
    // Client-side rendering control
    const [mounted, setMounted] = useState(false);
    useEffect(() => {
        setMounted(true);
    }, []);

    // Convert menu data to Antd Menu items
    const renderMenuItems = (menuItems: MenuDataItem[]): MenuItem[] => {
        return menuItems.map(item => {
            if (item.children && item.children.length > 0) {
                return {
                    key: item.path || item.name || '',
                    icon: item.icon,
                    label: item.name,
                    children: renderMenuItems(item.children)
                };
            }
            return {
                key: item.path || item.name || '',
                icon: item.icon,
                label: item.name,
                onClick: item.path ? () => router.push(item.path || '/') : undefined
            };
        });
    };

    const accessibleMenus = getAccessibleMenus(loginUser, menus);
    const menuItems = renderMenuItems(accessibleMenus);

    const handleLogoClick = () => {
        router.push("/");
    };

    const handleLoginClick = () => {
        router.push("/user/login");
    };

    return {
        mounted,
        pathname,
        loginUser,
        menuItems,
        handleLogoClick,
        handleLoginClick,
    };
}; 