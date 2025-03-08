"use client";
import { GithubFilled, LogoutOutlined, } from '@ant-design/icons';
import { ProLayout, } from '@ant-design/pro-components';
import { Dropdown, message, } from 'antd';
import React from 'react';
import Image from "next/image";
import { usePathname, useRouter } from "next/navigation";
import Link from "next/link";
import menus from "../../../config/menus";
import { AppDispatch, RootState } from "@/stores";
import { useDispatch, useSelector } from "react-redux";
import { setLoginUser } from "@/stores/loginUser";
import { DEFAULT_USER } from "@/constants/user";
import { UserOutlined } from "@ant-design/icons";
import getAccessibleMenus from "@/access/menuAccess";
import { apiAuthLogout } from '@/api/identity/api/auth';


interface Props {
    children: React.ReactNode;
}

export default function BasicLayout({ children }: Props) {
    const pathname = usePathname();
    const dispatch = useDispatch<AppDispatch>();

    const loginUser = useSelector((state: RootState) => state.loginUser);
    const router = useRouter();

    /**
     * User logout
     */
    const userLogout = async () => {
        try {
            await apiAuthLogout();
            message.success("Logout successfully");
            dispatch(setLoginUser(DEFAULT_USER));
            router.push("/user/login");
        } catch (e: any) {
            message.error(e.message);
        }
        return;
    };
    return (
        <div id="basicLayout">
            <ProLayout
                title="FireFly Articles"
                layout="top"
                logo={
                    <Image
                        src="/assets/logo.png"
                        height={32}
                        width={32}
                        alt="Firefly Bird"
                    />
                }
                location={{
                    pathname,
                }}
                avatarProps={{
                    src: loginUser.userAvatar || "/assets/logo.png",
                    size: "small",
                    title: loginUser.userName || "Bird",
                    render: (props, dom) => {
                        return loginUser.id ? (
                            <Dropdown
                                menu={{
                                    items: [
                                        // {
                                        //     key: "userCenter",
                                        //     icon: <UserOutlined />,
                                        //     label: "User Center",
                                        // },
                                        {
                                            key: "logout",
                                            icon: <LogoutOutlined />,
                                            label: "Logout",
                                        },
                                    ],
                                    onClick: async (event: { key: React.Key }) => {
                                        const { key } = event;
                                        if (key === "logout") {
                                            userLogout();
                                        } else if (key === "userCenter") {
                                            router.push("/user/center");
                                        }
                                    },
                                }}
                            >
                                {dom}
                            </Dropdown>

                        ) : (
                            <div onClick={() => router.push("/user/login")}>{dom}</div>
                        );
                    },
                }}
                actionsRender={(props) => {
                    if (props.isMobile) return [];
                    return [
                        <a
                            key="github"
                            href="https://github.com/FyreFly26710/FireflyArticles"
                            target="_blank"
                        >
                            <GithubFilled key="GithubFilled" />
                        </a>,];
                }}
                headerTitleRender={(logo, title, _) => {
                    const defaultDom = (
                        <a>
                            {logo}
                            {title}
                        </a>
                    );

                    if (_.isMobile) return defaultDom;
                    return (
                        <>
                            {defaultDom}
                        </>
                    );
                }}
                menuDataRender={() => {
                    return getAccessibleMenus(loginUser, menus);
                }}
                menuItemRender={(item, dom) => (
                    <Link href={item.path || "/"} target={item.target}>
                        {dom}
                    </Link>
                )}
                onMenuHeaderClick={(e) => router.push("/")}
            >

                {children}
            </ProLayout>
        </div>
    );
};