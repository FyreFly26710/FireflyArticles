"use client";

import { AntdRegistry } from "@ant-design/nextjs-registry";
import "./globals.css";
import BasicLayout from "@/layouts/BasicLayout";
import { useCallback, useEffect } from "react";
import { Provider, useDispatch } from "react-redux";
import store, { AppDispatch } from "@/stores";
import { setLoginUser } from "@/stores/loginUser";
import AccessLayout from "@/access/AccessLayout";
import enGB from 'antd/locale/en_GB';
import { ConfigProvider } from "antd";
import { apiAuthGetLoginUser } from "@/api/identity/api/auth";

const InitLayout: React.FC<Readonly<{ children: React.ReactNode; }>> = ({ children }) => {
    const dispatch = useDispatch<AppDispatch>();
    const doInitLoginUser = useCallback(async () => {
        try {
            const res = await apiAuthGetLoginUser();
            // @ts-ignore
            const user: API.LoginUserDto = res.data;
            if (user) {
                dispatch(setLoginUser(user));
            }
        } catch (error: any) {

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
