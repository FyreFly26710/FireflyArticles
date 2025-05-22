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

    const fetchArticles = async () => {
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
    };

    const initializeData = (data: {
        topics: API.TopicDto[];
        tags: API.TagDto[];
        topicsByCategory: Record<string, API.TopicDto[]>;
    }) => {
        dispatch(setInitialData(data));
    };

    const handleSearch = (keyword: string) => {
        dispatch(setKeywordFilter(keyword));
    };

    const handleClearSearch = () => {
        dispatch(setKeywordFilter(undefined));
    };

    const handleTopicChange = (topicIds: number[]) => {
        dispatch(setTopicFilter(topicIds));
    };

    const handleTagChange = (tagIds: number[]) => {
        dispatch(setTagFilter(tagIds));
    };

    const handlePageChange = (pageNumber: number, pageSize?: number) => {
        dispatch(setPagination({ pageNumber, pageSize }));
    };

    const handleRemoveTopicFilter = (topicId: number) => {
        dispatch(removeTopicFilter(topicId));
    };

    const handleRemoveTagFilter = (tagId: number) => {
        dispatch(removeTagFilter(tagId));
    };

    const handleClearFilters = () => {
        dispatch(clearFilters());
    };

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
