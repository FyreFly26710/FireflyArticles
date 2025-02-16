"use client";

import React from "react";
import { LoginForm, ProForm, ProFormText } from "@ant-design/pro-form";
import { message } from "antd";
import { LockOutlined, UserOutlined } from "@ant-design/icons";
import { useRouter } from "next/navigation";
import Link from "next/link";
import Image from "next/image";
import { AppDispatch } from "@/stores";
import { setLoginUser } from "@/stores/loginUser";
import { useDispatch } from "react-redux";
import "./index.css";
import {postUserLogin} from "@/api/identity/api/user";

/**
 * User login page
 * @param props
 */
const UserLoginPage: React.FC = (props) => {
    const [form] = ProForm.useForm();
    const router = useRouter();
    const dispatch = useDispatch<AppDispatch>();

    /**
     * Submit handler
     * @param values
     */
    const doSubmit = async (values: any) => {
        try {
            const res = await postUserLogin(values);
            if (res.data) {
                message.success("Login successful!");
                // Save user login state
                // @ts-ignore
                dispatch(setLoginUser(res.data));
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
                title="FF Articles - User Login"
                subTitle="Knowledge sharing platform"
                onFinish={doSubmit}
                submitter={{
                    searchConfig: {
                        submitText: "Login",
                    },
                }}
            >
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
                <div
                    style={{
                        marginBlockEnd: 24,
                        textAlign: "end",
                    }}
                >
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
