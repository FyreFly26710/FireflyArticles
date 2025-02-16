// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";

/** 此处后端没有提供注释 POST /api/identity/admin/delete */
export async function postAdminOpenApiDelete(
  body: number,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/identity/admin/delete", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 GET /api/identity/admin/get-dto/${param0} */
export async function getAdminGetDtoId(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.getAdminGetDtoIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.UserDtoApiResponse>(
    `/api/identity/admin/get-dto/${param0}`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 此处后端没有提供注释 POST /api/identity/admin/list */
export async function postAdminList(
  body: API.PageRequest,
  options?: { [key: string]: any }
) {
  return request<API.UserResponsePageResponseApiResponse>(
    "/api/identity/admin/list",
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      data: body,
      ...(options || {}),
    }
  );
}

/** 此处后端没有提供注释 POST /api/identity/admin/update */
export async function postAdminUpdate(
  body: API.UserUpdateRequest,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/identity/admin/update", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}
