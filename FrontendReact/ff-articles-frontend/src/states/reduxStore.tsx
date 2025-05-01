import { configureStore } from "@reduxjs/toolkit";
import loginUser from "@/states/loginUser";
import editArticle from "@/states/editArticle";
import articleModal from "@/states/articleModal";

const store = configureStore({
    reducer: {
        // store states
        loginUser,
        editArticle,
        articleModal,
    },
});

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch

export default store;
