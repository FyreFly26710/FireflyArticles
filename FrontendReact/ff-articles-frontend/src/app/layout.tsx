"use client";

import { AntdRegistry } from "@ant-design/nextjs-registry";
import "./globals.css";
import BasicLayout from "@/layouts/BasicLayout";
import { useCallback, useEffect } from "react";
import { Provider, useDispatch } from "react-redux";
import store, { AppDispatch } from "@/stores/reduxStore";
import { setLoginUser } from "@/stores/loginUser";
import AccessLayout from "@/access/AccessLayout";
import enGB from 'antd/locale/en_GB';
import { ConfigProvider } from "antd";
import { storage } from "@/stores/storage";
import { apiAuthGetLoginUser } from "@/api/identity/api/auth";

const InitLayout: React.FC<Readonly<{ children: React.ReactNode; }>> = ({ children }) => {
    const dispatch = useDispatch<AppDispatch>();

    const doInitLoginUser = useCallback(async () => {
        try {
            let user = storage.getUser();
            // if user is not in localStorage, get user from backend
            // Todo: use guest user if user is not in localStorage
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

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode; }>) {
    return (
        <html lang="en">
            <body>
                <ConfigProvider locale={enGB}>
                    <AntdRegistry>
                        <Provider store={store}>
                            <InitLayout>
                                <BasicLayout>
                                    <AccessLayout>{children}</AccessLayout>
                                </BasicLayout>
                            </InitLayout>
                        </Provider>
                    </AntdRegistry>
                </ConfigProvider>
            </body>
        </html>
    );
}
