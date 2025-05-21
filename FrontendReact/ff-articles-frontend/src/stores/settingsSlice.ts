import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { SettingsState, LayoutSettings, ChatDisplaySettings, ChatBehaviorSettings, SelectedModel } from "@/types";

const defaultLayoutSettings: LayoutSettings = {
    sidebarCollapsed: false,
    rightSidebarCollapsed: false,
};

const defaultChatDisplaySettings: ChatDisplaySettings = {
    showMessageTimestamp: true,
    showModelName: true,
    showTokenUsed: true,
    showTimeTaken: true,
    enableMarkdownRendering: true,
    showOnlyActiveMessages: false,
    enableCollapsibleMessages: true,
};

const defaultChatBehaviorSettings: ChatBehaviorSettings = {
    enableThinking: true,
    enableStreaming: true,
    selectedModel: {
        providerName: 'deepseek',
        model: 'deepseek-chat'
    }
};

const initialState: SettingsState = {
    layout: defaultLayoutSettings,
    chatDisplay: defaultChatDisplaySettings,
    chatBehavior: defaultChatBehaviorSettings,
    chatProviders: null,
};

export const settingsSlice = createSlice({
    name: "settings",
    initialState,
    reducers: {
        // Layout Settings
        setLayoutSettings: (state, action: PayloadAction<LayoutSettings>) => {
            state.layout = action.payload;
        },
        toggleSidebar: (state) => {
            state.layout.sidebarCollapsed = !state.layout.sidebarCollapsed;
        },
        toggleRightSidebar: (state) => {
            state.layout.rightSidebarCollapsed = !state.layout.rightSidebarCollapsed;
        },

        // Chat Display Settings
        setChatDisplaySettings: (state, action: PayloadAction<ChatDisplaySettings>) => {
            state.chatDisplay = action.payload;
        },
        toggleChatDisplaySetting: (state, action: PayloadAction<keyof ChatDisplaySettings>) => {
            const key = action.payload;
            if (typeof state.chatDisplay[key] === 'boolean') {
                state.chatDisplay[key] = !state.chatDisplay[key];
            }
        },

        // Chat Behavior Settings
        setChatBehaviorSettings: (state, action: PayloadAction<ChatBehaviorSettings>) => {
            state.chatBehavior = action.payload;
        },
        setSelectedModel: (state, action: PayloadAction<SelectedModel>) => {
            state.chatBehavior.selectedModel = action.payload;
        },
        toggleChatBehaviorSetting: (state, action: PayloadAction<keyof Omit<ChatBehaviorSettings, 'selectedModel'>>) => {
            const key = action.payload;
            if (typeof state.chatBehavior[key] === 'boolean') {
                state.chatBehavior[key] = !state.chatBehavior[key];
            }
        },

        // Chat Providers
        setChatProviders: (state, action: PayloadAction<API.ChatProvider[]>) => {
            state.chatProviders = action.payload;
        },

        // Reset all settings to default
        resetSettings: (state) => {
            state.layout = defaultLayoutSettings;
            state.chatDisplay = defaultChatDisplaySettings;
            state.chatBehavior = defaultChatBehaviorSettings;
            state.chatProviders = null;
        },
    },
});

export const {
    setLayoutSettings,
    toggleSidebar,
    toggleRightSidebar,
    setChatDisplaySettings,
    toggleChatDisplaySetting,
    setChatBehaviorSettings,
    setSelectedModel,
    toggleChatBehaviorSetting,
    setChatProviders,
    resetSettings,
} = settingsSlice.actions; 