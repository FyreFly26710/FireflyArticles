'use client';

import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { apiArticleGetByPage } from '@/api/contents/api/article';
import { message } from 'antd';

interface ArticlesContextType {
  // Articles data
  articles: API.ArticleDto[];
  loading: boolean;
  total: number;

  // Topics and Tags data
  topics: API.TopicDto[];
  topicsByCategory: Record<string, API.TopicDto[]>;
  tags: API.TagDto[];

  // Pagination
  pageNumber: number;
  pageSize: number;

  // Filters
  filters: {
    keyword?: string;
    topicIds?: number[];
    tagIds?: number[];
  };

  // Actions
  handleSearch: (keyword: string) => void;
  handleClearSearch: () => void;
  handleTopicChange: (topicIds: number[]) => void;
  handleTagChange: (tagIds: number[]) => void;
  handleClearFilters: () => void;
  handlePageChange: (page: number, pageSize?: number) => void;
  removeTopicFilter: (topicId: number) => void;
  removeTagFilter: (tagId: number) => void;
  refresh: () => Promise<void>;
}

const defaultValue: ArticlesContextType = {
  articles: [],
  loading: true,
  total: 0,
  topics: [],
  topicsByCategory: {},
  tags: [],
  pageNumber: 1,
  pageSize: 10,
  filters: {},
  handleSearch: () => { },
  handleClearSearch: () => { },
  handleTopicChange: () => { },
  handleTagChange: () => { },
  handleClearFilters: () => { },
  handlePageChange: () => { },
  removeTopicFilter: () => { },
  removeTagFilter: () => { },
  refresh: async () => { },
};

const ArticlesContext = createContext<ArticlesContextType>(defaultValue);

export const useArticlesContext = () => useContext(ArticlesContext);

interface ArticlesProviderProps {
  children: ReactNode;
  initialTopics: API.TopicDto[];
  initialTags: API.TagDto[];
  initialTopicsByCategory: Record<string, API.TopicDto[]>;
}

export const ArticlesProvider = ({
  children,
  initialTopics,
  initialTags,
  initialTopicsByCategory,
}: ArticlesProviderProps) => {
  const [articles, setArticles] = useState<API.ArticleDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [total, setTotal] = useState(0);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [filters, setFilters] = useState<{
    keyword?: string;
    topicIds?: number[];
    tagIds?: number[];
  }>({});

  const fetchArticles = async () => {
    setLoading(true);
    try {
      const response = await apiArticleGetByPage({
        PageNumber: pageNumber,
        PageSize: pageSize,
        IncludeUser: false,
        DisplaySubArticles: true,
        Keyword: filters.keyword,
        TopicIds: filters.topicIds,
        TagIds: filters.tagIds,
        SortByRelevance: true,
      });

      if (response.data) {
        setArticles(response.data?.data || []);
        setTotal(response.data?.counts || 0);
      }
    } catch (error) {
      console.error("Failed to fetch articles:", error);
      message.error("Failed to load articles. Please try again later.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchArticles();
  }, [pageNumber, pageSize, filters]);

  const handleSearch = (keyword: string) => {
    setPageNumber(1);
    setFilters(prev => ({ ...prev, keyword }));
  };

  const handleClearSearch = () => {
    setPageNumber(1);
    setFilters(prev => ({ ...prev, keyword: undefined }));
  };

  const handleTopicChange = (topicIds: number[]) => {
    setPageNumber(1);
    setFilters(prev => ({ ...prev, topicIds }));
  };

  const handleTagChange = (tagIds: number[]) => {
    setPageNumber(1);
    setFilters(prev => ({ ...prev, tagIds }));
  };

  const handleClearFilters = () => {
    setPageNumber(1);
    setFilters({});
  };

  const handlePageChange = (page: number, size?: number) => {
    setPageNumber(page);
    if (size) setPageSize(size);
  };

  const removeTopicFilter = (topicId: number) => {
    if (!filters.topicIds) return;

    const newTopicIds = filters.topicIds.filter(id => id !== topicId);
    setFilters(prev => ({
      ...prev,
      topicIds: newTopicIds.length ? newTopicIds : undefined
    }));
  };

  const removeTagFilter = (tagId: number) => {
    if (!filters.tagIds) return;

    const newTagIds = filters.tagIds.filter(id => id !== tagId);
    setFilters(prev => ({
      ...prev,
      tagIds: newTagIds.length ? newTagIds : undefined
    }));
  };

  const value = {
    articles,
    loading,
    total,
    topics: initialTopics,
    topicsByCategory: initialTopicsByCategory,
    tags: initialTags,
    pageNumber,
    pageSize,
    filters,
    handleSearch,
    handleClearSearch,
    handleTopicChange,
    handleTagChange,
    handleClearFilters,
    handlePageChange,
    removeTopicFilter,
    removeTagFilter,
    refresh: fetchArticles,
  };

  return (
    <ArticlesContext.Provider value={value}>
      {children}
    </ArticlesContext.Provider>
  );
}; 