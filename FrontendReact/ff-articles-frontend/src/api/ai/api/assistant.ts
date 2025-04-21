/* eslint-disable */
import request from "@/libs/request";

/** GET /api/ai/assistant/providers */
export async function apiAiAssistantProviders(
  options?: { [key: string]: any }
) {
  return request<API.ChatProviderListApiResponse>("/api/ai/assistants/providers", {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    timeout: 5 * 60 * 1000,
    ...(options || {}),
  });
}