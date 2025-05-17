/* eslint-disable */
import request from "@/libs/utils/request";

/** POST /api/ai/articles-prompts/generate-article-list */
export async function apiAiArticlesPromptsGenerateList(
  body: API.ArticleListRequest,
  options?: { [key: string]: any }
) {
  return request<API.MessageDtoListApiResponse>("/api/ai/articles-prompts/generate-article-list", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** POST /api/ai/articles-prompts/regenerate-article-list */
export async function apiAiArticlesPromptsRegenerateList(
  body: API.ExistingArticleListRequest,
  options?: { [key: string]: any }
) {
  return request<API.MessageDtoListApiResponse>("/api/ai/articles-prompts/regenerate-article-list", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** POST /api/ai/articles-prompts/generate-article-content */
export async function apiAiArticlesPromptsGenerateContent(
  body: API.ContentRequest,
  options?: { [key: string]: any }
) {
  return request<API.MessageDtoListApiResponse>("/api/ai/articles-prompts/generate-article-content", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}
/** POST /api/ai/articles-prompts/regenerate-article-content */
export async function apiAiArticlesPromptsRegenerateContent(
  body: API.RegenerateArticleContentRequest,
  options?: { [key: string]: any }
) {
  return request<API.MessageDtoListApiResponse>("/api/ai/articles-prompts/regenerate-article-content", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}
