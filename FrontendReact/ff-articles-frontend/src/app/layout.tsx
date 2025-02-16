"use client";

import {AntdRegistry} from "@ant-design/nextjs-registry";
import "./globals.css";
import BasicLayout from "@/layouts/BasicLayout";
import {useCallback, useEffect} from "react";
import {Provider, useDispatch} from "react-redux";
import store, {AppDispatch} from "@/stores";
import {postUserGetLoginUser} from "@/api/identity/api/user";
import {setLoginUser} from "@/stores/loginUser";
import AccessLayout from "@/access/AccessLayout";

const InitLayout: React.FC<
    Readonly<{
        children: React.ReactNode;
    }>
> = ({children}) => {
    const dispatch = useDispatch<AppDispatch>();
    // Initialize global user state
    const doInitLoginUser = useCallback(async () => {

        // Get user information
        const res = await postUserGetLoginUser();
        if (res.data) {
            // @ts-ignore
            dispatch(setLoginUser(res.data));
        } else {
        }
    }, []);

    useEffect(() => {
        doInitLoginUser();
    }, []);

    return <>{children}</>;
};

export default function RootLayout({
                                       children,
                                   }: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <html lang="en">
        <body>
        <AntdRegistry>
            <Provider store={store}>s
                <InitLayout>
                    <BasicLayout>
                        <AccessLayout>{children}</AccessLayout>
                    </BasicLayout>
                </InitLayout>
            </Provider>
        </AntdRegistry>
        </body>
        </html>
    );
}
