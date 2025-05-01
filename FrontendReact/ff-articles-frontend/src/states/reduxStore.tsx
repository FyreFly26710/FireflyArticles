import { configureStore } from "@reduxjs/toolkit";
import loginUser from "@/states/loginUser";

const store = configureStore({
    reducer: {
        // store states
        loginUser,
    },
});

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch

export default store;
