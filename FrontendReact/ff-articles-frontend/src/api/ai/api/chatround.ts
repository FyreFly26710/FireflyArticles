/* eslint-disable */
import request from "@/libs/request";

// Import the base URL configuration
const DevBaseUrl = "https://localhost:21000/";
const ProdBaseUrl = process.env.NEXT_PUBLIC_BASE_URL;
const baseUrl = ProdBaseUrl ?? DevBaseUrl;

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
    timeout: 10*60*1000,
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
    onInit?: (data: any) => void;
    onChunk?: (content: string) => void;
    onDone?: (data: API.ChatRoundDto) => void;
    onError?: (error: any) => void;
    onTokens?: (promptTokens: number, completionTokens: number) => void;
  }
) {
  // Ensure streaming is enabled
  body.enableStreaming = true;
  
  // Create a new AbortController to cancel the request if needed
  const controller = new AbortController();
  console.log("Streaming request started");
  
  // Make a fetch request to the streaming endpoint
  fetch(`${baseUrl}api/ai/chat-rounds/stream`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
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
          // Handle different event types
          switch (eventType) {
            case "init":
              callbacks.onInit?.(JSON.parse(data));
              break;
            case "chunk":
              callbacks.onChunk?.(data);
              break;
            case "done":
              callbacks.onDone?.(JSON.parse(data));
              break;
            case "error":
              callbacks.onError?.(data);
              break;
          }
        } else if (data) {
          // Handle raw SSE data without event type (DeepSeek format)
          if (data === "[DONE]") {
            // End of stream
            break;
          }
          
          try {
            const parsedData = JSON.parse(data);
            
            // Check if it's a DeepSeek completion chunk
            if (parsedData.choices && parsedData.choices.length > 0) {
              const choice = parsedData.choices[0];
              
              // Handle token counts when they appear in the final message
              if (parsedData.usage) {
                callbacks.onTokens?.(
                  parsedData.usage.prompt_tokens || 0,
                  parsedData.usage.completion_tokens || 0
                );
              }
              
              // Extract content if available
              if (choice.delta && choice.delta.content) {
                callbacks.onChunk?.(choice.delta.content);
              }
            }
          } catch (e) {
            console.error("Error parsing SSE data:", e);
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

/** PUT /api/ai/chat-rounds */
export async function apiChatRoundUpdateByRequest(
  body: API.ChatRoundReQueryRequest,
  options?: { [key: string]: any }
) {
  return request<API.ChatRoundDtoApiResponse>("/api/ai/chat-rounds", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    timeout: 5*60*1000,
    ...(options || {}),
  });
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
