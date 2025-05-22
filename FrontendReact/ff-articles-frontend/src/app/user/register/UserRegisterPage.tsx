"use client";

import React from "react";
import { LoginForm, ProForm, ProFormText } from "@ant-design/pro-form";
import { LockOutlined, UserOutlined } from "@ant-design/icons";
import Link from "next/link";
import Image from "next/image";
import { useUserActions } from "@/hooks/useUserActions";

/**
 * User registration page
 * @param props
 */
const UserRegisterPage: React.FC = () => {
    const [form] = ProForm.useForm();
    const { handleRegister } = useUserActions();

    /**
     * Submit handler
     * @param values
     */
    const doSubmit = async (values: API.UserRegisterRequest) => {
        const success = await handleRegister(values);
        if (success) {
            form.resetFields();
        }
    };

    return (
        <div id="userRegisterPage">
            <LoginForm<API.UserRegisterRequest>
                form={form}
                logo={
                    <Image src="/assets/logo.png" alt="FF Articles" width={44} height={44} />
                }
                title="FF Articles - User Registration"
                subTitle="Knowledge sharing platform"
                onFinish={doSubmit}
                submitter={{
                    searchConfig: {
                        submitText: "Register",
                    },
                }}
            >
                <ProFormText
                    fieldProps={{
                        size: "large",
                        prefix: <UserOutlined />,
                    }}
                    name="userAccount"
                    placeholder={"Please enter your username"}
                    rules={[
                        {
                            required: true,
                            message: "Please enter your username!",
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
                <ProFormText.Password
                    name="checkPassword"
                    fieldProps={{
                        size: "large",
                        prefix: <LockOutlined />,
                    }}
                    placeholder={"Confirm your password"}
                    rules={[
                        {
                            required: true,
                            message: "Please enter your password again!",
                        },
                    ]}
                />
                <div
                    style={{
                        marginBlockEnd: 24,
                        textAlign: "end",
                    }}
                >
                    Already have an account?{" "}
                    <Link prefetch={false} href={"/user/login"}>
                        Login
                    </Link>
                </div>
            </LoginForm>
        </div>
    );
};

export default UserRegisterPage;
