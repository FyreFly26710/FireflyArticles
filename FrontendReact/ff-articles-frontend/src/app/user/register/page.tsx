"use client";

import React from "react";
import { LoginForm, ProForm, ProFormText } from "@ant-design/pro-form";
import { message } from "antd";
import { LockOutlined, UserOutlined } from "@ant-design/icons";
import Link from "next/link";
import { useRouter } from "next/navigation";
import Image from "next/image";
import "./index.css";
import {postUserRegister} from "@/api/identity/api/user";

/**
 * User registration page
 * @param props
 */
const UserRegisterPage: React.FC = (props) => {
    const [form] = ProForm.useForm();
    const router = useRouter();

    /**
     * Submit handler
     * @param values
     */
    const doSubmit = async (values: any) => {
        try {
            const res = await postUserRegister(values);
            if (res.data) {
                message.success("Registration successful, please log in");
                // Go to login page
                router.push("/user/login");
            }
        } catch (e:any) {
            message.error("Registration failed, " + e.message);
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
