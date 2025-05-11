"use client";

import { AntdRegistry } from "@ant-design/nextjs-registry";
import "./globals.css";
import AppLayout from "@/layouts/AppLayout";
import { Provider, useDispatch, useSelector } from "react-redux";
import store, { AppDispatch, RootState } from "@/states/reduxStore";
import enGB from 'antd/locale/en_GB';
import { ConfigProvider } from "antd";
import Forbidden from "./forbidden";
import { checkAccess } from "@/libs/utils/accessUtils";
import { findAllMenuItemByPath } from "../../config/menus";
import { usePathname } from "next/navigation";
import AccessEnum from "@/libs/constants/accessEnum";
import { useEffect } from "react";
import { storage } from "@/states/localStorage";
import { apiAuthGetLoginUser } from "@/api/identity/api/auth";
import { useCallback } from "react";
import { setLoginUser } from "@/states/reduxStore";


const UserStateLayout: React.FC<Readonly<{ children: React.ReactNode; }>> = ({ children }) => {
    const dispatch = useDispatch<AppDispatch>();

    const doInitLoginUser = useCallback(async () => {
        try {
            let user = storage.getUser();
            if (!user) {
                const res = await apiAuthGetLoginUser();
                user = res.data ?? null;

                if (user) {
                    storage.setUser(user);
                }
            }

            if (user) {
                dispatch(setLoginUser(user));
            }
        } catch (error: any) {
            console.error('Failed to initialize user:', error);
        }
    }, []);

    useEffect(() => {
        doInitLoginUser();
    }, []);

    return <>{children}</>;
};

const AccessLayout: React.FC<
    Readonly<{
        children: React.ReactNode;
    }>
> = ({ children }) => {
    const pathname = usePathname();
    const loginUser = useSelector((state: RootState) => state.loginUser);
    // auth check
    const menu = findAllMenuItemByPath(pathname) || {};
    const needAccess = menu?.access ?? AccessEnum.NOT_LOGIN;
    const canAccess = checkAccess(loginUser, needAccess);
    if (!canAccess) {
        return <Forbidden />;
    }
    return <>{children}</>;
};


export default function RootLayout({ children }: Readonly<{ children: React.ReactNode; }>) {
    return (
        <html lang="en">
            <body>
                <ConfigProvider locale={enGB}>
                    <AntdRegistry>
                        <Provider store={store}>
                            <UserStateLayout>
                                <AppLayout>
                                    <AccessLayout>{children}</AccessLayout>
                                </AppLayout>
                            </UserStateLayout>
                        </Provider>
                    </AntdRegistry>
                </ConfigProvider>
            </body>
        </html>
    );
}
