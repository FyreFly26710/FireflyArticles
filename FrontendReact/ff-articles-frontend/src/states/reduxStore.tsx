import { configureStore } from "@reduxjs/toolkit";
import loginUser from "@/states/loginUser";
import editArticle from "@/states/editArticle";

const store = configureStore({
    reducer: {
        // store states
        loginUser,
        editArticle,
    },
});

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch

export default store;
