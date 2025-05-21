import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface ArticleEditState {
    isEditing: boolean;
    currentArticle: API.ArticleDto | null;
}

const initialState: ArticleEditState = {
    isEditing: false,
    currentArticle: null,
};

export const articleEditSlice = createSlice({
    name: "articleEdit",
    initialState,
    reducers: {
        startEditing: (state, action: PayloadAction<API.ArticleDto>) => {
            state.isEditing = true;
            state.currentArticle = action.payload;
        },
        updateEditingArticle: (state, action: PayloadAction<Partial<API.ArticleDto>>) => {
            if (state.currentArticle) {
                state.currentArticle = { ...state.currentArticle, ...action.payload };
            }
        },
        cancelEditing: (state) => {
            state.isEditing = false;
            state.currentArticle = null;
        },
    },
});

export const { startEditing, updateEditingArticle, cancelEditing } = articleEditSlice.actions; 