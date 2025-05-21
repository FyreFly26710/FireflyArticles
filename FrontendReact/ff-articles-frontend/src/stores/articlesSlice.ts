import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface ArticlesState {
    articles: API.ArticleDto[];
    loading: boolean;
    total: number;
    topics: API.TopicDto[];
    topicsByCategory: Record<string, API.TopicDto[]>;
    tags: API.TagDto[];
    pagination: {
        pageNumber: number;
        pageSize: number;
    };
    filters: {
        keyword?: string;
        topicIds?: number[];
        tagIds?: number[];
    };
}

const initialState: ArticlesState = {
    articles: [],
    loading: false,
    total: 0,
    topics: [],
    topicsByCategory: {},
    tags: [],
    pagination: {
        pageNumber: 1,
        pageSize: 10
    },
    filters: {}
};

export const articlesSlice = createSlice({
    name: "articles",
    initialState,
    reducers: {
        setArticles: (state, action: PayloadAction<API.ArticleDto[]>) => {
            state.articles = action.payload;
        },
        setLoading: (state, action: PayloadAction<boolean>) => {
            state.loading = action.payload;
        },
        setTotal: (state, action: PayloadAction<number>) => {
            state.total = action.payload;
        },
        setInitialData: (state, action: PayloadAction<{
            topics: API.TopicDto[];
            tags: API.TagDto[];
            topicsByCategory: Record<string, API.TopicDto[]>;
        }>) => {
            state.topics = action.payload.topics;
            state.tags = action.payload.tags;
            state.topicsByCategory = action.payload.topicsByCategory;
        },
        setPagination: (state, action: PayloadAction<{ pageNumber: number; pageSize?: number }>) => {
            state.pagination.pageNumber = action.payload.pageNumber;
            if (action.payload.pageSize) {
                state.pagination.pageSize = action.payload.pageSize;
            }
        },
        setKeywordFilter: (state, action: PayloadAction<string | undefined>) => {
            state.filters.keyword = action.payload;
            state.pagination.pageNumber = 1;
        },
        setTopicFilter: (state, action: PayloadAction<number[]>) => {
            state.filters.topicIds = action.payload;
            state.pagination.pageNumber = 1;
        },
        setTagFilter: (state, action: PayloadAction<number[]>) => {
            state.filters.tagIds = action.payload;
            state.pagination.pageNumber = 1;
        },
        removeTopicFilter: (state, action: PayloadAction<number>) => {
            if (state.filters.topicIds) {
                state.filters.topicIds = state.filters.topicIds.filter(id => id !== action.payload);
                if (state.filters.topicIds.length === 0) {
                    state.filters.topicIds = undefined;
                }
            }
        },
        removeTagFilter: (state, action: PayloadAction<number>) => {
            if (state.filters.tagIds) {
                state.filters.tagIds = state.filters.tagIds.filter(id => id !== action.payload);
                if (state.filters.tagIds.length === 0) {
                    state.filters.tagIds = undefined;
                }
            }
        },
        clearFilters: (state) => {
            state.filters = {};
            state.pagination.pageNumber = 1;
        }
    }
});

export const {
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
    clearFilters
} = articlesSlice.actions; 