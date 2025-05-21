import { useCallback } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { message } from 'antd';
import { apiArticleGetByPage } from '@/api/contents/api/article';
import { RootState } from '@/stores';
import {
    setArticles,
    setLoading,
    setTotal,
    setInitialData,
    setPagination,
    setKeywordFilter,
    setTopicFilter,
    setTagFilter,
    removeTopicFilter,
    removeTagFilter,
    clearFilters,
} from '@/stores/articlesSlice';

export const useArticles = () => {
    const dispatch = useDispatch();
    const {
        articles,
        loading,
        total,
        topics,
        topicsByCategory,
        tags,
        pagination,
        filters,
    } = useSelector((state: RootState) => state.articles);

    const fetchArticles = useCallback(async () => {
        dispatch(setLoading(true));
        try {
            const response = await apiArticleGetByPage({
                PageNumber: pagination.pageNumber,
                PageSize: pagination.pageSize,
                IncludeUser: false,
                DisplaySubArticles: true,
                Keyword: filters.keyword,
                TopicIds: filters.topicIds,
                TagIds: filters.tagIds,
                SortByRelevance: true,
            });

            if (response.data) {
                dispatch(setArticles(response.data.data || []));
                dispatch(setTotal(response.data.counts || 0));
            }
        } catch (error) {
            console.error("Failed to fetch articles:", error);
            message.error("Failed to load articles. Please try again later.");
        } finally {
            dispatch(setLoading(false));
        }
    }, [dispatch, pagination.pageNumber, pagination.pageSize, filters]);

    const initializeData = useCallback((data: {
        topics: API.TopicDto[];
        tags: API.TagDto[];
        topicsByCategory: Record<string, API.TopicDto[]>;
    }) => {
        dispatch(setInitialData(data));
    }, [dispatch]);

    const handleSearch = useCallback((keyword: string) => {
        dispatch(setKeywordFilter(keyword));
    }, [dispatch]);

    const handleClearSearch = useCallback(() => {
        dispatch(setKeywordFilter(undefined));
    }, [dispatch]);

    const handleTopicChange = useCallback((topicIds: number[]) => {
        dispatch(setTopicFilter(topicIds));
    }, [dispatch]);

    const handleTagChange = useCallback((tagIds: number[]) => {
        dispatch(setTagFilter(tagIds));
    }, [dispatch]);

    const handlePageChange = useCallback((pageNumber: number, pageSize?: number) => {
        dispatch(setPagination({ pageNumber, pageSize }));
    }, [dispatch]);

    const handleRemoveTopicFilter = useCallback((topicId: number) => {
        dispatch(removeTopicFilter(topicId));
    }, [dispatch]);

    const handleRemoveTagFilter = useCallback((tagId: number) => {
        dispatch(removeTagFilter(tagId));
    }, [dispatch]);

    const handleClearFilters = useCallback(() => {
        dispatch(clearFilters());
    }, [dispatch]);

    return {
        // State
        articles,
        loading,
        total,
        topics,
        topicsByCategory,
        tags,
        pageNumber: pagination.pageNumber,
        pageSize: pagination.pageSize,
        filters,

        // Actions
        fetchArticles,
        initializeData,
        handleSearch,
        handleClearSearch,
        handleTopicChange,
        handleTagChange,
        handlePageChange,
        handleRemoveTopicFilter,
        handleRemoveTagFilter,
        handleClearFilters,
    };
};
