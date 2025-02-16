// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";

/** 此处后端没有提供注释 POST /api/contents/tag/add */
export async function postTagAdd(
  body: API.TagAddRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/contents/tag/add", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/contents/tag/delete */
export async function postTagOpenApiDelete(
  body: API.DeleteByIdRequest,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/contents/tag/delete", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/contents/tag/edit */
export async function postTagEdit(
  body: API.TagEditRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/contents/tag/edit", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 GET /api/contents/tag/get/${param0} */
export async function getTagGetId(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.getTagGetIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.TagResponseApiResponse>(
    `/api/contents/tag/get/${param0}`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 此处后端没有提供注释 GET /api/contents/tag/get/all */
export async function getTagGetAll(options?: { [key: string]: any }) {
  return request<API.TagResponseListApiResponse>("/api/contents/tag/get/all", {
    method: "GET",
    ...(options || {}),
  });
}
