// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";

/** 此处后端没有提供注释 GET /api/identity/users */
export async function apiUserGetByPage(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.apiUserGetByPageParams,
  options?: { [key: string]: any }
) {
  return request<API.UserDtoPagedApiResponse>("/api/identity/users", {
    method: "GET",
    params: {
      ...params,
    },
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/identity/users */
export async function apiUserUpdateByRequest(
  body: API.UserUpdateRequest,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/identity/users", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 GET /api/identity/users/${param0} */
export async function apiUserGetById(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.apiUserGetByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.UserApiDtoApiResponse>(`/api/identity/users/${param0}`, {
    method: "GET",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 DELETE /api/identity/users/${param0} */
export async function apiUserDeleteById(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.apiUserDeleteByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.BooleanApiResponse>(`/api/identity/users/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}
