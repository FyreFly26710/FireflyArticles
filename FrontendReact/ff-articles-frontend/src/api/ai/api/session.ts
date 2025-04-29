/* eslint-disable */
import request from "@/libs/utils/request";
import qs from "qs";

/** GET /api/ai/sessions/${param0} */
export async function apiSessionGetById(
  params: API.apiSessionGetByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.SessionDtoApiResponse>(
    `/api/ai/sessions/${param0}`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** GET /api/ai/sessions */
export async function apiSessionGetSessions(
  params: API.SessionQueryRequest,
  options?: { [key: string]: any }
) {
  return request<API.SessionDtoListApiResponse>("/api/ai/sessions", {
    method: "GET",
    params: {
      includeChatRounds: params.includeChatRounds
    },
    ...(options || {}),
  });
}

/** PUT /api/ai/sessions */
export async function apiSessionUpdate(
  body: API.SessionEditRequest,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/ai/sessions", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** DELETE /api/ai/sessions/${param0} */
export async function apiSessionDeleteById(
  params: API.apiSessionDeleteByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.BooleanApiResponse>(`/api/ai/sessions/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}
