import { useState, useEffect, useCallback } from 'react';
import { apiArticleGetByPage } from '@/api/contents/api/article';
import { message } from 'antd';

interface ArticleTableFilter {
  keyword?: string;
  topicIds?: number[];
  tagIds?: number[];
}

interface ArticleTableState {
  articles: API.ArticleDto[];
  loading: boolean;
  total: number;
  pageNumber: number;
  pageSize: number;
  filters: ArticleTableFilter;
}

export const useArticleTable = () => {
  const [state, setState] = useState<ArticleTableState>({
    articles: [],
    loading: true,
    total: 0,
    pageNumber: 1,
    pageSize: 10,
    filters: {}
  });

  const fetchArticles = useCallback(async () => {
    setState(prev => ({ ...prev, loading: true }));
    try {
      const response = await apiArticleGetByPage({
        PageNumber: state.pageNumber,
        PageSize: state.pageSize,
        IncludeUser: true,
        DisplaySubArticles: true,
        Keyword: state.filters.keyword,
        TopicIds: state.filters.topicIds,
        TagIds: state.filters.tagIds,
        SortByRelevance: true,
      });

      if (response.data) {
        setState(prev => ({
          ...prev,
          articles: response.data?.data || [],
          total: response.data?.counts || 0,
          loading: false
        }));
      }
    } catch (error) {
      console.error("Failed to fetch articles:", error);
      message.error("Failed to load articles. Please try again later.");
      setState(prev => ({ ...prev, loading: false }));
    }
  }, [state.pageNumber, state.pageSize, state.filters]);

  useEffect(() => {
    fetchArticles();
  }, [fetchArticles]);

  const handleSearch = (keyword: string) => {
    setState(prev => ({
      ...prev,
      pageNumber: 1,
      filters: { ...prev.filters, keyword }
    }));
  };

  const handleClearSearch = () => {
    setState(prev => ({
      ...prev,
      pageNumber: 1,
      filters: { ...prev.filters, keyword: undefined }
    }));
  };

  const handleTopicChange = (topicIds: number[]) => {
    setState(prev => ({
      ...prev,
      pageNumber: 1,
      filters: { ...prev.filters, topicIds }
    }));
  };

  const handleTagChange = (tagIds: number[]) => {
    setState(prev => ({
      ...prev,
      pageNumber: 1,
      filters: { ...prev.filters, tagIds }
    }));
  };

  const handleClearFilters = () => {
    setState(prev => ({
      ...prev,
      pageNumber: 1,
      filters: {}
    }));
  };

  const handlePageChange = (page: number, pageSize?: number) => {
    setState(prev => ({
      ...prev,
      pageNumber: page,
      pageSize: pageSize || prev.pageSize
    }));
  };

  const removeTopicFilter = (topicId: number) => {
    if (!state.filters.topicIds) return;
    
    const newTopicIds = state.filters.topicIds.filter(id => id !== topicId);
    setState(prev => ({
      ...prev,
      filters: { ...prev.filters, topicIds: newTopicIds.length ? newTopicIds : undefined }
    }));
  };

  const removeTagFilter = (tagId: number) => {
    if (!state.filters.tagIds) return;
    
    const newTagIds = state.filters.tagIds.filter(id => id !== tagId);
    setState(prev => ({
      ...prev,
      filters: { ...prev.filters, tagIds: newTagIds.length ? newTagIds : undefined }
    }));
  };

  return {
    articles: state.articles,
    loading: state.loading,
    total: state.total,
    pageNumber: state.pageNumber,
    pageSize: state.pageSize,
    filters: state.filters,
    handleSearch,
    handleClearSearch,
    handleTopicChange,
    handleTagChange,
    handleClearFilters,
    handlePageChange,
    removeTopicFilter,
    removeTagFilter,
    refresh: fetchArticles
  };
};
