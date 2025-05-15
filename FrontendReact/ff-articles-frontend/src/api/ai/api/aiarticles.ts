/* eslint-disable */
import request from "@/libs/utils/request";

/** POST /api/ai/articles/generate-article-list */
export async function apiAiArticlesGenerateList(
  body: API.ArticleListRequest,
  options?: { [key: string]: any }
) {
  return request<API.ArticlesAIResponseDto>("/api/ai/articles/generate-article-list", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    timeout: 5 * 60 * 1000,
    ...(options || {}),
  });
}

/** POST /api/ai/articles/generate-article-content */
export async function apiAiArticlesGenerateContent(
  body: API.ContentRequest,
  options?: { [key: string]: any }
) {
  return request<API.NumberApiResponse>("/api/ai/articles/generate-article-content", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}
