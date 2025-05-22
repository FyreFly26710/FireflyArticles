"use client";

import React from "react";
import { LoginForm, ProForm, ProFormText } from "@ant-design/pro-form";
import { Divider, message } from "antd";
import { LockOutlined, UserOutlined } from "@ant-design/icons";
import Link from "next/link";
import Image from "next/image";
import GoogleLoginButton from "@/components/shared/GmailLoginBtn";
import { useUserActions } from "@/hooks/useUserActions";

/**
 * User login page
 * @param props
 */
const UserLoginPage: React.FC = () => {
    const [form] = ProForm.useForm();
    const { handleLogin } = useUserActions();

    /**
     * Submit handler
     * @param values
     */
    const doSubmit = async (values: API.UserLoginRequest) => {
        const success = await handleLogin(values);
        if (success) {
            form.resetFields();
        }
    };

    return (
        <div id="userLoginPage">
            <LoginForm<API.UserLoginRequest>
                form={form}
                logo={
                    <Image src="/assets/logo.png" alt="FF Articles" width={44} height={44} />
                }
                title="FF Articles - Login"
                subTitle="Knowledge sharing platform"
                onFinish={doSubmit}
                submitter={{
                    searchConfig: {
                        submitText: "Login",
                    },
                }}
            >
                <GoogleLoginButton />
                <Divider>Or</Divider>
                <ProFormText
                    name="userAccount"
                    fieldProps={{
                        size: "large",
                        prefix: <UserOutlined />,
                    }}
                    placeholder={"Please enter your user account"}
                    allowClear={false}
                    rules={[
                        {
                            required: true,
                            message: "Please enter your user account!",
                        },
                    ]}
                />
                <ProFormText.Password
                    name="userPassword"
                    fieldProps={{
                        size: "large",
                        prefix: <LockOutlined />,
                    }}
                    placeholder={"Please enter your password"}
                    rules={[
                        {
                            required: true,
                            message: "Please enter your password!",
                        },
                    ]}
                />

                <div style={{ marginBlockEnd: 24, textAlign: "end", }}>
                    Don&#39;t have an account yet?{" "}
                    <Link prefetch={false} href={"/user/register"}>
                        Register
                    </Link>
                </div>
            </LoginForm>
        </div>
    );
};

export default UserLoginPage;
