// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";

/** 此处后端没有提供注释 POST /api/identity/user/getLoginUser */
export async function postUserGetLoginUser(options?: { [key: string]: any }) {
  return request<API.LoginUserResponseApiResponse>(
    "/api/identity/user/getLoginUser",
    {
      method: "POST",
      ...(options || {}),
    }
  );
}

/** 此处后端没有提供注释 POST /api/identity/user/login */
export async function postUserLogin(
  body: API.UserLoginRequest,
  options?: { [key: string]: any }
) {
  return request<API.LoginUserResponseApiResponse>("/api/identity/user/login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/identity/user/logout */
export async function postUserLogout(options?: { [key: string]: any }) {
  return request<API.BooleanApiResponse>("/api/identity/user/logout", {
    method: "POST",
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/identity/user/register */
export async function postUserRegister(
  body: API.UserRegisterRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/identity/user/register", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}
