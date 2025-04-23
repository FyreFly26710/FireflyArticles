/* eslint-disable */
import request, { url } from "@/libs/request";


/** POST /api/ai/chat-rounds */
export async function apiChatRoundAddByRequest(
  body: API.ChatRoundCreateRequest,
  options?: { [key: string]: any }
) {
  return request<API.ChatRoundDtoApiResponse>("/api/ai/chat-rounds", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    timeout: 10 * 60 * 1000,
    ...(options || {}),
  });
}

/** 
 * POST /api/ai/chat-rounds/stream 
 * This function creates an EventSource to handle streaming responses
 */
export function apiChatRoundStreamResponse(
  body: API.ChatRoundCreateRequest,
  callbacks: {
    onInit?: (data: API.ChatRoundDto) => void;
    onChunk?: (content: string) => void;
    onDone?: (data: API.ChatRoundDto) => void;
    onError?: (error: any) => void;
  }
) {
  // Create a new AbortController to cancel the request if needed
  const controller = new AbortController();
  console.log("Streaming request started");

  // Ensure we don't have double slashes in the URL
  const baseUrl = url.endsWith('/') ? url.slice(0, -1) : url;
  const streamUrl = `${baseUrl}/api/ai/chat-rounds/stream`;

  console.log("Using stream URL:", streamUrl);

  // Make a fetch request to the streaming endpoint
  fetch(streamUrl, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
    body: JSON.stringify(body),
    signal: controller.signal,
  })
    .then(response => {
      if (!response.ok) {
        console.log(response);
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const reader = response.body!.getReader();
      const decoder = new TextDecoder();
      let buffer = "";

      function processChunk({ done, value }: { done: boolean, value?: Uint8Array }): Promise<void> | void {
        if (done) {
          // Handle any remaining buffer data
          if (buffer.length > 0) {
            processBuffer(buffer);
          }
          return;
        }

        // Decode the chunk and add it to our buffer
        const chunk = decoder.decode(value, { stream: true });
        buffer += chunk;

        // Process any complete events in the buffer
        buffer = processBuffer(buffer);

        // Continue reading
        return reader.read().then(processChunk);
      }

      function processBuffer(inputBuffer: string): string {
        // Split the buffer by double newlines (SSE format)
        const events = inputBuffer.split("\n\n");

        // The last element might be incomplete, so we keep it in the buffer
        const remainingBuffer = events.pop() || "";

        // Process each complete event
        for (const event of events) {
          if (!event.trim()) continue;

          const lines = event.split("\n");
          let eventType = "";
          let data = "";

          for (const line of lines) {
            if (line.startsWith("event: ")) {
              eventType = line.slice(7);
            } else if (line.startsWith("data: ")) {
              data = line.slice(6);
            }
          }

          if (eventType && data) {
            // Handle different event types based on the updated backend
            switch (eventType) {
              case "start":
                try {
                  const initData = JSON.parse(data);
                  callbacks.onInit?.(initData);
                } catch (e) {
                  console.error("Error parsing init event data:", e);
                }
                break;
              case "generate":
                // The backend now sends the actual content directly
                callbacks.onChunk?.(data);
                break;
              case "finish":
                try {
                  const doneData = JSON.parse(data);
                  callbacks.onDone?.(doneData);
                } catch (e) {
                  console.error("Error parsing done event data:", e);
                }
                break;
              case "error":
                callbacks.onError?.(data);
                break;
            }
          }
        }

        return remainingBuffer;
      }

      // Start reading
      reader.read().then(processChunk).catch(error => {
        callbacks.onError?.(error);
      });
    })
    .catch(error => {
      callbacks.onError?.(error);
    });

  // Return a function to abort the request
  return () => {
    controller.abort();
  };
}

/** DELETE /api/ai/chat-rounds/${param0} */
export async function apiChatRoundDeleteById(
  params: API.apiChatRoundDeleteByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.BooleanApiResponse>(`/api/ai/chat-rounds/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** PUT /api/ai/chat-rounds/disable */
export async function apiChatRoundDisable(
  body: number[],
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/ai/chat-rounds/disable", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}
/** PUT /api/ai/chat-rounds/enable */
export async function apiChatRoundEnable(
  body: number[],
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/ai/chat-rounds/enable", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** DELETE /api/ai/chat-rounds */
export async function apiChatRoundDeleteByIds(
  body: number[],
  options?: { [key: string]: any }
) {
  return request<API.BooleanApiResponse>("/api/ai/chat-rounds", {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}
