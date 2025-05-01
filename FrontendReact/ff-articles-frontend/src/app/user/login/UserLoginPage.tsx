"use client";

import React from "react";
import { LoginForm, ProForm, ProFormText } from "@ant-design/pro-form";
import { Divider, message } from "antd";
import { LockOutlined, UserOutlined } from "@ant-design/icons";
import { useRouter } from "next/navigation";
import Link from "next/link";
import Image from "next/image";
import { AppDispatch } from "@/states/reduxStore";
import { setLoginUser } from "@/states/loginUser";
import { useDispatch } from "react-redux";
import { apiAuthLogin } from "@/api/identity/api/auth";
import GoogleLoginButton from "@/components/shared/GmailLoginBtn";
import { storage } from "@/states/storage";

/**
 * User login page
 * @param props
 */
const UserLoginPage: React.FC = () => {
    const [form] = ProForm.useForm();
    const router = useRouter();
    const dispatch = useDispatch<AppDispatch>();

    /**
     * Submit handler
     * @param values
     */
    const doSubmit = async (values: any) => {
        try {
            const res = await apiAuthLogin(values);
            if (res.data) {
                message.success("Login successful!");
                // Save user to both Redux and localStorage
                dispatch(setLoginUser(res.data));
                storage.setUser(res.data);
                router.replace("/");
                form.resetFields();
            }
        } catch (e: any) {
            message.error('Login failed, ' + e.message);
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
