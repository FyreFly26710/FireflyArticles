'use client';
import React, { createContext, useState, useContext, ReactNode } from 'react';

interface ArticleGenerationState {
  [sortNumber: number]: {
    articleId?: number;
    loading: boolean;
    error?: string;
  };
}

interface AiGenContextType {
  loading: boolean;
  setLoading: (loading: boolean) => void;
  responseData: string;
  setResponseData: (data: string) => void;
  parsedArticles: API.ArticlesAIResponse | null;
  setParsedArticles: (articles: API.ArticlesAIResponse | null) => void;
  error: string | null;
  setError: (error: string | null) => void;
  clearResults: () => void;
  articleListRequest: API.ArticleListRequest | null;
  setArticleListRequest: (request: API.ArticleListRequest | null) => void;
  generationState: ArticleGenerationState;
  setArticleGenerationState: (sortNumber: number, state: { articleId?: number; loading: boolean; error?: string }) => void;
  resetArticleGenerationState: () => void;
}

const AiGenContext = createContext<AiGenContextType | undefined>(undefined);

export const AiGenProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [loading, setLoading] = useState(false);
  const [responseData, setResponseData] = useState('');
  const [parsedArticles, setParsedArticles] = useState<API.ArticlesAIResponse | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [articleListRequest, setArticleListRequest] = useState<API.ArticleListRequest | null>(null);
  const [generationState, setGenerationState] = useState<ArticleGenerationState>({});

  const clearResults = () => {
    setResponseData('');
    setParsedArticles(null);
    setError(null);
    setArticleListRequest(null);
    resetArticleGenerationState();
  };

  const setArticleGenerationState = (
    sortNumber: number,
    state: { articleId?: number; loading: boolean; error?: string }
  ) => {
    setGenerationState(prev => ({
      ...prev,
      [sortNumber]: {
        ...prev[sortNumber],
        ...state
      }
    }));
  };

  const resetArticleGenerationState = () => {
    setGenerationState({});
  };

  return (
    <AiGenContext.Provider
      value={{
        loading,
        setLoading,
        responseData,
        setResponseData,
        parsedArticles,
        setParsedArticles,
        error,
        setError,
        clearResults,
        articleListRequest,
        setArticleListRequest,
        generationState,
        setArticleGenerationState,
        resetArticleGenerationState
      }}
    >
      {children}
    </AiGenContext.Provider>
  );
};

export const useAiGenContext = () => {
  const context = useContext(AiGenContext);
  if (context === undefined) {
    throw new Error('useAiGenContext must be used within an AiGenProvider');
  }
  return context;
};
