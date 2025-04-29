// @ts-ignore
/* eslint-disable */
import request from "@/libs/utils/request";

/**GET /api/contents/topics */
export async function apiTopicGetByPage(
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

/**PUT /api/contents/topics */
export async function apiTopicAddByRequest(
  body: API.TopicAddRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/contents/topics", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/**POST /api/contents/topics */
export async function apiTopicEditByRequest(
  body: API.TopicEditRequest,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/contents/topics", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/**GET /api/contents/topics/${param0} */
export async function apiTopicGetById(
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

/**DELETE /api/contents/topics/${param0} */
export async function apiTopicDeleteById(
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
