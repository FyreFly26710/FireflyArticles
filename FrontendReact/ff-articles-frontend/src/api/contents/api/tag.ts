// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";

/** 此处后端没有提供注释 GET /api/contents/tags */
export async function apiTagGetAll(options?: { [key: string]: any }) {
  return request<API.TagDtoListApiResponse>("/api/contents/tags", {
    method: "GET",
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 PUT /api/contents/tags */
export async function apiTagAddByRequest(
  body: API.TagAddRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/contents/tags", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/contents/tags */
export async function apiTagEditByRequest(
  body: API.TagEditRequest,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/contents/tags", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 GET /api/contents/tags/${param0} */
export async function apiTagGetById(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.apiTagGetByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.TagDtoApiResponse>(`/api/contents/tags/${param0}`, {
    method: "GET",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 DELETE /api/contents/tags/${param0} */
export async function apiTagDeleteById(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.apiTagDeleteByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.BooleanApiResponse>(`/api/contents/tags/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}
