// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";

/** 此处后端没有提供注释 GET /api/contents/topics */
export async function apiTopicGetByPage(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.apiTopicGetByPageParams,
  options?: { [key: string]: any }
) {
  return request<API.TopicDtoPagedApiResponse>("/api/contents/topics", {
    method: "GET",
    params: {
      ...params,
    },
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 PUT /api/contents/topics */
export async function apiTopicAddByRequest(
  body: API.TopicAddRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/contents/topics", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/contents/topics */
export async function apiTopicEditByRequest(
  body: API.TopicEditRequest,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/contents/topics", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 GET /api/contents/topics/${param0} */
export async function apiTopicGetById(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.apiTopicGetByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.TopicDtoApiResponse>(`/api/contents/topics/${param0}`, {
    method: "GET",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 DELETE /api/contents/topics/${param0} */
export async function apiTopicDeleteById(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.apiTopicDeleteByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.BooleanApiResponse>(`/api/contents/topics/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}
