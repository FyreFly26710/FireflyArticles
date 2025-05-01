import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface ArticleModalState {
    isVisible: boolean;
    currentArticle: API.ArticleDto | null;
}

const initialState: ArticleModalState = {
    isVisible: false,
    currentArticle: null,
};

const articleModalSlice = createSlice({
    name: "articleModal",
    initialState,
    reducers: {
        showModal: (state, action: PayloadAction<API.ArticleDto>) => {
            state.isVisible = true;
            state.currentArticle = action.payload;
        },
        hideModal: (state) => {
            state.isVisible = false;
        },
    },
});

export const { showModal, hideModal } = articleModalSlice.actions;
export default articleModalSlice.reducer; 