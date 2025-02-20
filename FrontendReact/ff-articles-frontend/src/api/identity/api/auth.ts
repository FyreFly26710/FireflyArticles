// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";

/** 此处后端没有提供注释 POST /api/identity/auth/getLoginUser */
export async function apiAuthGetLoginUser(options?: { [key: string]: any }) {
  return request<API.LoginUserDtoApiResponse>(
    "/api/identity/auth/getLoginUser",
    {
      method: "POST",
      ...(options || {}),
    }
  );
}

/** 此处后端没有提供注释 POST /api/identity/auth/login */
export async function apiAuthLogin(
  body: API.UserLoginRequest,
  options?: { [key: string]: any }
) {
  return request<API.LoginUserDtoApiResponse>("/api/identity/auth/login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/identity/auth/logout */
export async function apiAuthLogout(options?: { [key: string]: any }) {
  return request<API.BooleanApiResponse>("/api/identity/auth/logout", {
    method: "POST",
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/identity/auth/register */
export async function apiAuthRegister(
  body: API.UserRegisterRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/identity/auth/register", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}
