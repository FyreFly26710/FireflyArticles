import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface ArticleGenerationState {
    [sortNumber: number]: {
        articleId?: number;
        loading: boolean;
        error?: string;
    };
}

interface AiGenState {
    loading: boolean;
    responseData: string;
    parsedArticles: API.ArticlesAIResponse | null;
    articleListRequest: API.ArticleListRequest | null;
    generationState: ArticleGenerationState;
}

const initialState: AiGenState = {
    loading: false,
    responseData: '',
    parsedArticles: null,
    articleListRequest: null,
    generationState: {},
};

export const aiGenSlice = createSlice({
    name: "aiGen",
    initialState,
    reducers: {
        setLoading: (state, action: PayloadAction<boolean>) => {
            state.loading = action.payload;
        },
        setResponseData: (state, action: PayloadAction<string>) => {
            state.responseData = action.payload;
        },
        setParsedArticles: (state, action: PayloadAction<API.ArticlesAIResponse | null>) => {
            state.parsedArticles = action.payload;
        },
        setArticleListRequest: (state, action: PayloadAction<API.ArticleListRequest | null>) => {
            state.articleListRequest = action.payload;
        },
        setArticleGenerationState: (
            state,
            action: PayloadAction<{
                sortNumber: number;
                state: { articleId?: number; loading: boolean; error?: string };
            }>
        ) => {
            const { sortNumber, state: newState } = action.payload;
            state.generationState[sortNumber] = {
                ...state.generationState[sortNumber],
                ...newState,
            };
        },
        clearResults: (state) => {
            state.responseData = '';
            state.parsedArticles = null;
            state.articleListRequest = null;
            state.generationState = {};
        },
        resetArticleGenerationState: (state) => {
            state.generationState = {};
        },
    },
});

export const {
    setLoading,
    setResponseData,
    setParsedArticles,
    setArticleListRequest,
    setArticleGenerationState,
    clearResults,
    resetArticleGenerationState,
} = aiGenSlice.actions; 