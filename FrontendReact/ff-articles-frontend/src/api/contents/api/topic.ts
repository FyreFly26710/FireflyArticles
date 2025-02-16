// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";

/** 此处后端没有提供注释 POST /api/contents/topic/add */
export async function postTopicAdd(
  body: API.TopicAddRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/contents/topic/add", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/contents/topic/delete */
export async function postTopicOpenApiDelete(
  body: API.DeleteByIdRequest,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/contents/topic/delete", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/contents/topic/edit */
export async function postTopicEdit(
  body: API.TopicEditRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/contents/topic/edit", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/contents/topic/get-page */
export async function postTopicGetPage(
  body: API.PageRequest,
  options?: { [key: string]: any }
) {
  return request<API.TopicResponsePageResponseApiResponse>(
    "/api/contents/topic/get-page",
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

/** 此处后端没有提供注释 GET /api/contents/topic/get/${param0} */
export async function getTopicGetId(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.getTopicGetIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.TopicResponseApiResponse>(
    `/api/contents/topic/get/${param0}`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}
