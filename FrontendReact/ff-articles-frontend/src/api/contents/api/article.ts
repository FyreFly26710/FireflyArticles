// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";

/** 此处后端没有提供注释 GET /api/contents/articles */
export async function apiArticleGetByPage(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.apiArticleGetByPageParams,
  options?: { [key: string]: any }
) {
  return request<API.ArticleDtoPagedApiResponse>("/api/contents/articles", {
    method: "GET",
    params: {
      ...params,
    },
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 PUT /api/contents/articles */
export async function apiArticleAddByRequest(
  body: API.ArticleAddRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/contents/articles", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/contents/articles */
export async function apiArticleEditByRequest(
  body: API.ArticleEditRequest,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/contents/articles", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 GET /api/contents/articles/${param0} */
export async function apiArticleGetById(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.apiArticleGetByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ArticleDtoApiResponse>(
    `/api/contents/articles/${param0}`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 此处后端没有提供注释 DELETE /api/contents/articles/${param0} */
export async function apiArticleDeleteById(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.apiArticleDeleteByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.BooleanApiResponse>(`/api/contents/articles/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}
