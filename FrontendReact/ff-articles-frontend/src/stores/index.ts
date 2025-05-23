import { configureStore } from "@reduxjs/toolkit";
import { persistStore, persistReducer, PersistConfig } from "redux-persist";
import { loginUserSlice } from "./loginUserSlice";
import { articleEditSlice } from "./articleEditSlice";
import { settingsSlice } from "./settingsSlice";
import { aiGenSlice } from "./aiGenSlice";
import { articlesSlice } from "./articlesSlice";
import { chatSlice } from "./chatSlice";
import { SettingsState } from "@/types";

// Create a custom storage object that loads storage only on client side
const createNoopStorage = () => {
    return {
        getItem(_key: string) {
            return Promise.resolve(null);
        },
        setItem(_key: string, value: any) {
            return Promise.resolve(value);
        },
        removeItem(_key: string) {
            return Promise.resolve();
        },
    };
};
// Fix storage cannot be imported in docker build
const storage = typeof window !== 'undefined' 
    ? require('redux-persist/lib/storage').default 
    : createNoopStorage();


// Configure persist for login user
const loginUserPersistConfig: PersistConfig<API.LoginUserDto> = {
    key: "loginUser",
    storage,
    whitelist: ["id", "username", "email", "role"]
};

// Configure persist for settings
const settingsPersistConfig: PersistConfig<SettingsState> = {
    key: "settings",
    storage,
    whitelist: ["layout", "chatDisplay", "chatBehavior", "chatProviders"]
};

// Create the store
const store = configureStore({
    reducer: {
        loginUser: persistReducer<API.LoginUserDto>(loginUserPersistConfig, loginUserSlice.reducer),
        articleEdit: articleEditSlice.reducer,
        settings: persistReducer<SettingsState>(settingsPersistConfig, settingsSlice.reducer),
        aiGen: aiGenSlice.reducer,
        articles: articlesSlice.reducer,
        chat: chatSlice.reducer,
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware({
            serializableCheck: {
                ignoredActions: ["persist/PERSIST", "persist/REHYDRATE"],
            },
        }),
});

// Create persistor
export const persistor = persistStore(store);

// Export types
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export default store;
