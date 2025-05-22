"use client";

import { AntdRegistry } from "@ant-design/nextjs-registry";
import "./globals.css";
import AppLayout from "@/layouts/AppLayout";
import { Provider, useDispatch, useSelector } from "react-redux";
import store, { persistor } from "@/stores";
import enGB from 'antd/locale/en_GB';
import { ConfigProvider } from "antd";
import { PersistGate } from 'redux-persist/integration/react';
import Forbidden from "./forbidden";
import { checkAccess } from "@/libs/utils/accessUtils";
import { findAllMenuItemByPath } from "../../config/menus";
import { usePathname } from "next/navigation";
import AccessEnum from "@/libs/constants/accessEnum";
import { useEffect } from "react";
import { apiAuthGetLoginUser } from "@/api/identity/api/auth";
import { useCallback } from "react";
import { setLoginUser } from "@/stores/loginUserSlice";
import type { AppDispatch, RootState } from "@/stores";

const UserStateLayout: React.FC<Readonly<{ children: React.ReactNode; }>> = ({ children }) => {
    const dispatch = useDispatch<AppDispatch>();

    const initializeUser = useCallback(async () => {
        try {
            const res = await apiAuthGetLoginUser();
            if (res.data) {
                dispatch(setLoginUser(res.data));
            }
        } catch (error: any) {
            console.error('Failed to initialize user:', error);
        }
    }, [dispatch]);

    useEffect(() => {
        initializeUser();
    }, [initializeUser]);

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
    return <div className="h-full">{children}</div>;
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode; }>) {
    return (
        <html lang="en">
            <body>
                <ConfigProvider locale={enGB}>
                    <AntdRegistry>
                        <Provider store={store}>
                            <PersistGate loading={null} persistor={persistor}>
                                <UserStateLayout>
                                    <AppLayout>
                                        <AccessLayout>{children}</AccessLayout>
                                    </AppLayout>
                                </UserStateLayout>
                            </PersistGate>
                        </Provider>
                    </AntdRegistry>
                </ConfigProvider>
            </body>
        </html>
    );
}
