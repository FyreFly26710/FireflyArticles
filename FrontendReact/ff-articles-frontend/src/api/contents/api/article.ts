// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";

/** 此处后端没有提供注释 POST /api/contents/article/add */
export async function postArticleAdd(
  body: API.ArticleAddRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/contents/article/add", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/contents/article/delete */
export async function postArticleOpenApiDelete(
  body: API.DeleteByIdRequest,
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/contents/article/delete", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/contents/article/edit */
export async function postArticleEdit(
  body: API.ArticleEditRequest,
  options?: { [key: string]: any }
) {
  return request<API.Int32ApiResponse>("/api/contents/article/edit", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 POST /api/contents/article/get-page */
export async function postArticleGetPage(
  body: API.PageRequest,
  options?: { [key: string]: any }
) {
  return request<API.ArticleResponsePageResponseApiResponse>(
    "/api/contents/article/get-page",
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

/** 此处后端没有提供注释 GET /api/contents/article/get/${param0} */
export async function getArticleGetId(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.getArticleGetIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ArticleResponseApiResponse>(
    `/api/contents/article/get/${param0}`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}
