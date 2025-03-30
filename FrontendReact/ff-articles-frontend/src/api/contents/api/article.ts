// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";
import qs from "qs";

/**GET /api/contents/articles */
export async function apiArticleGetByPage(
  params: API.apiArticleGetByPageParams,
  options?: { [key: string]: any }
) {
  return request<API.ArticleDtoPagedApiResponse>("/api/contents/articles", {
    method: "GET",
    params: {
      ...params,
    },    
    paramsSerializer: (params) => {
      return qs.stringify(params, {
        arrayFormat: 'repeat',
        encode: false,
        indices: false
      });
    },
    ...(options || {}),
  });
}

/**PUT /api/contents/articles */
export async function apiArticleAddByRequest(
  body: API.ArticleAddRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/contents/articles", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/**POST /api/contents/articles */
export async function apiArticleEditByRequest(
  body: API.ArticleEditRequest,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/contents/articles", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/**GET /api/contents/articles/${param0} */
export async function apiArticleGetById(
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

/**DELETE /api/contents/articles/${param0} */
export async function apiArticleDeleteById(
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
