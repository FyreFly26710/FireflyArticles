"use client";

import { AntdRegistry } from "@ant-design/nextjs-registry";
import "./globals.css";
import BasicLayout from "@/layouts/appLayouts/BasicLayout";
import { Provider } from "react-redux";
import store from "@/stores/reduxStore";
import enGB from 'antd/locale/en_GB';
import { ConfigProvider } from "antd";
import AccessLayout from "@/layouts/appLayouts/AccessLayout";
import UserStateLayout from "@/layouts/appLayouts/UserStateLayout";



export default function RootLayout({ children }: Readonly<{ children: React.ReactNode; }>) {
    return (
        <html lang="en">
            <body>
                <ConfigProvider locale={enGB}>
                    <AntdRegistry>
                        <Provider store={store}>
                            <UserStateLayout>
                                <BasicLayout>
                                    <AccessLayout>{children}</AccessLayout>
                                </BasicLayout>
                            </UserStateLayout>
                        </Provider>
                    </AntdRegistry>
                </ConfigProvider>
            </body>
        </html>
    );
}
