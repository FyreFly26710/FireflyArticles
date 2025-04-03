declare namespace API {
  // Chat round API params
  type apiChatRoundDeleteByIdParams = {
    id: number;
  };

  // Session API params
  type apiSessionGetByIdParams = {
    id: number;
  };

  type apiSessionDeleteByIdParams = {
    id: number;
  };

  // Chat round models
  type ChatRoundCreateRequest = {
    sessionId: number;
    historyChatRoundIds?: number[];
    userMessage: string;
    model?: string;
    enableStreaming?: boolean;
  };

  type ChatRoundReQueryRequest = {
    chatRoundId: number;
    sessionId: number;
    userMessage: string;
    model?: string;
    includeHistory?: boolean;
  };

  type ChatRoundDto = {
    sessionId: number;
    chatRoundId: number;
    userMessage: string;
    assistantMessage: string;
    model: string;
    createdAt: string;
    promptTokens: number;
    completionTokens: number;
    totalTokens: number;
    timeTaken: number;
    isActive: boolean;
  };

  // Session models
  type SessionQueryRequest = {
    includeChatRounds?: boolean;
  };

  type SessionEditRequest = {
    sessionId: number;
    sessionName?: string;
  };

  type SessionDto = {
    sessionId: number;
    sessionName: string;
    rounds: ChatRoundDto[];
    roundCount: number;
    createdAt: string;
  };

  // API responses
  type ChatRoundDtoApiResponse = {
    code?: number;
    message?: string;
    data?: ChatRoundDto;
  };

  type SessionDtoApiResponse = {
    code?: number;
    message?: string;
    data?: SessionDto;
  };

  type SessionDtoListApiResponse = {
    code?: number;
    message?: string;
    data?: SessionDto[];
  };

  type BooleanApiResponse = {
    code?: number;
    message?: string;
    data?: boolean;
  };
}
