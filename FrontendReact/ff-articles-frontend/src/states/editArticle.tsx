import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface EditArticleState {
    isEditing: boolean;
    currentArticle: API.ArticleDto | null;
}

const initialState: EditArticleState = {
    isEditing: false,
    currentArticle: null,
};

const editArticleSlice = createSlice({
    name: "editArticle",
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

export const { startEditing, updateEditingArticle, cancelEditing } = editArticleSlice.actions;
export default editArticleSlice.reducer; 