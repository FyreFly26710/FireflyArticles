"use client";
import { GithubFilled, LogoutOutlined } from '@ant-design/icons';
import { Layout, Menu, Dropdown, message, Space, Avatar, Typography } from 'antd';
import React, { useState, useEffect } from 'react';
import Image from "next/image";
import { usePathname, useRouter } from "next/navigation";
import Link from "next/link";
import menus from "../../config/menus";
import { AppDispatch, RootState } from "@/stores/reduxStore";
import { useDispatch, useSelector } from "react-redux";
import { setLoginUser } from "@/stores/loginUser";
import { DEFAULT_USER } from "@/libs/constants/user";
import { apiAuthLogout } from '@/api/identity/api/auth';
import { storage } from "@/stores/storage";
import { getAccessibleMenus } from '@/libs/utils/accessUtils';
import { MenuProps } from 'antd';
import { MenuDataItem } from "@ant-design/pro-components";

const { Header, Content } = Layout;
const { Title } = Typography;

interface Props {
    children: React.ReactNode;
}

type MenuItem = Required<MenuProps>['items'][number];

export default function AppLayout({ children }: Props) {
    const pathname = usePathname();
    const dispatch = useDispatch<AppDispatch>();
    const loginUser = useSelector((state: RootState) => state.loginUser);
    const router = useRouter();

    // Force client-side only rendering to avoid hydration mismatch
    const [mounted, setMounted] = useState(false);
    useEffect(() => {
        setMounted(true);
    }, []);

    const userLogout = async () => {
        try {
            await apiAuthLogout();
            message.success("Logout successfully");
            // Clear user from both Redux and localStorage
            storage.clearUser();
            dispatch(setLoginUser(DEFAULT_USER));
            router.push("/user/login");
        } catch (e: any) {
            message.error(e.message);
        }
        return;
    };

    // Return a simpler layout during server-side rendering to avoid hydration mismatch
    if (!mounted) {
        return (
            <div id="appLayout">
                <div style={{ minHeight: '100vh' }}>
                    {children}
                </div>
            </div>
        );
    }

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
                label: <Link href={item.path || "/"}>{item.name}</Link>
            };
        });
    };

    const accessibleMenus = getAccessibleMenus(loginUser, menus);
    const menuItems = renderMenuItems(accessibleMenus);

    return (
        <div id="appLayout">
            <Layout style={{ minHeight: '100vh' }}>
                <Header 
                    style={{ 
                        position: 'fixed', 
                        zIndex: 10, 
                        width: '100%', 
                        background: '#fff',
                        padding: '0 12px',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'space-between',
                        boxShadow: '0 2px 8px rgba(0, 0, 0, 0.08)',
                        height: 64
                    }}
                >
                    <div className="flex items-center">
                        <div 
                            className="flex items-center cursor-pointer" 
                            onClick={() => router.push("/")}
                            style={{ marginRight: '40px' }}
                        >
                            <Image
                                src="/assets/logo.png"
                                height={32}
                                width={32}
                                alt="Firefly Bird"
                            />
                            <Title 
                                level={4} 
                                style={{ 
                                    margin: 0, 
                                    marginLeft: '12px', 
                                    color: '#1890ff',
                                }}
                            >
                                FireFly Articles
                            </Title>
                        </div>
                        <Menu
                            mode="horizontal"
                            selectedKeys={[pathname]}
                            style={{ 
                                flex: 1, 
                                minWidth: '700px',
                                border: 'none',
                                background: 'transparent',
                                fontSize: '16px',
                            }}
                            items={menuItems}
                        />
                    </div>
                    <Space size="large">
                        <a
                            href="https://github.com/FyreFly26710/FireflyArticles"
                            target="_blank"
                            style={{ 
                                fontSize: '20px',
                                color: '#1890ff' 
                            }}
                        >
                            <GithubFilled />
                        </a>
                        {loginUser.id ? (
                            <Dropdown
                                menu={{
                                    items: [
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
                                        }
                                    },
                                }}
                            >
                                <Space className="cursor-pointer">
                                    <Avatar 
                                        src={loginUser.userAvatar || "/assets/logo.png"} 
                                        size="default"
                                        style={{ 
                                            border: '1px solid #f0f0f0',
                                        }}
                                    />
                                    <span style={{ fontWeight: '500' }}>
                                        {loginUser.userName || "Bird"}
                                    </span>
                                </Space>
                            </Dropdown>
                        ) : (
                            <Space 
                                className="cursor-pointer" 
                                onClick={() => router.push("/user/login")}
                                style={{
                                    background: '#f0f0f0',
                                    padding: '6px 12px',
                                    borderRadius: '4px',
                                    transition: 'all 0.3s',
                                }}
                                onMouseEnter={(e) => {
                                    e.currentTarget.style.background = '#e0e0e0';
                                }}
                                onMouseLeave={(e) => {
                                    e.currentTarget.style.background = '#f0f0f0';
                                }}
                            >
                                <Avatar 
                                    src="/assets/logo.png" 
                                    size="small"
                                />
                                <span>Login</span>
                            </Space>
                        )}
                    </Space>
                </Header>
                <Layout style={{ marginTop: 64 }}>
                    <Content style={{ margin: '24px 16px', overflow: 'initial', padding: '24px', background: '#fff', borderRadius: '4px', boxShadow: '0 1px 4px rgba(0,0,0,0.05)' }}>
                        {children}
                    </Content>
                </Layout>
            </Layout>
        </div>
    );
}