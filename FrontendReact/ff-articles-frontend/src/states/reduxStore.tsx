import { configureStore, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { DEFAULT_USER } from "@/libs/constants/user";

// Login User Slice
const loginUserSlice = createSlice({
    name: "loginUser",
    initialState: DEFAULT_USER,
    reducers: {
        setLoginUser: (state, action: PayloadAction<API.LoginUserDto>) => {
            return {
                ...action.payload,
            };
        },
    },
});

// Edit Article Slice
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


// Store Configuration
const store = configureStore({
    reducer: {
        loginUser: loginUserSlice.reducer,
        editArticle: editArticleSlice.reducer,
    },
});


// Actions
export const { setLoginUser } = loginUserSlice.actions;
export const { startEditing, updateEditingArticle, cancelEditing } = editArticleSlice.actions;

// Types
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export default store;

