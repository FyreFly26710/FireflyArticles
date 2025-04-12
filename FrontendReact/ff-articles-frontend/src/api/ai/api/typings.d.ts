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
    SessionTimeStamp: number;
    historyChatRoundIds?: number[];
    userMessage: string;
    model?: string;
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
    promptTokens: number;
    completionTokens: number;
    totalTokens: number;
    timeTaken: number;
    isActive: boolean;
    createTime: string;
    updateTime: string;
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
    timestamp: number;
    sessionName: string;
    rounds: ChatRoundDto[];
    roundCount: number;
    createTime: string;
    updateTime: string;
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

  // AI Articles models
  type ArticleListRequest = {
    topic: string;
    articleCount?: number;
  };

  type ArticlesAIResponseDto = {
    code?: number;
    message?: string;
    data?: {
      articles: AIGenArticleDto[];
      aiMessage: string;
    };
  };

  type AIGenArticleDto = {
    id: number;
    title: string;
    abstract: string;
    tags: string[];
  };
}
