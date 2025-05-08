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
    provider?: string;
  };

  type ChatRoundDto = {
    sessionId: number;
    chatRoundId: number;
    userMessage: string;
    assistantMessage: string;
    model: string;
    provider: string;
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
  type NumberApiResponse = {
    code?: number;
    message?: string;
    data?: number;
  };

  // AI Articles models
  type ArticleListRequest = {
    topic: string;
    topicAbstract: string;
    category: string;
    articleCount?: number;
    model: string;
    provider: string;
  };

  type ArticlesAIResponseDto = {
    code?: number;
    message?: string;
    data?: ArticlesAIResponse;
  };
  type ArticlesAIResponse = {
    articles: AIGenArticleDto[];
    aiMessage: string;
    topicId: number;
    category: string;
  };
  type AIGenArticleDto = {
    sortNumber: number;
    title: string;
    abstract: string;
    tags: string[];
  };
  type ContentRequest = {
    // id: number;
    sortNumber: number;
    topicId: number;
    topic: string;
    topicAbstract: string;
    category: string;
    title: string;
    abstract: string;
    tags: string[];
    model: string;
    provider: string;
  };
  type ChatProvider = {
    providerName: string;
    models: string[];
    isAvailable: boolean;
  };
  type ChatProviderListApiResponse = {
    code?: number;
    message?: string;
    data?: ChatProvider[];
  };
}
