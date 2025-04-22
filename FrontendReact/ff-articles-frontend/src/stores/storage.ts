const STORAGE_KEY_USER = 'user-storage';
const STORAGE_KEY_LAYOUT = 'layout-settings';
const STORAGE_KEY_CHAT_DISPLAY = 'chat-display-settings';
const STORAGE_KEY_CHAT_BEHAVIOR = 'chat-behavior-settings';
const STORAGE_KEY_CHAT_PROVIDERS = 'chat-providers';

// Layout Settings
export interface LayoutSettings {
    sidebarCollapsed: boolean;
    rightSidebarCollapsed: boolean;
}

const defaultLayoutSettings: LayoutSettings = {
    sidebarCollapsed: false,
    rightSidebarCollapsed: false,
};

// Chat Display Settings
export interface ChatDisplaySettings {
    showMessageTimestamp: boolean;
    showModelName: boolean;
    showTokenUsed: boolean;
    showTimeTaken: boolean;
    enableMarkdownRendering: boolean;
    showOnlyActiveMessages: boolean;
    enableCollapsibleMessages: boolean;
}

const defaultChatDisplaySettings: ChatDisplaySettings = {
    showMessageTimestamp: true,
    showModelName: true,
    showTokenUsed: true,
    showTimeTaken: true,
    enableMarkdownRendering: true,
    showOnlyActiveMessages: false,
    enableCollapsibleMessages: true,
};

// Chat Behavior Settings
export interface ChatBehaviorSettings {
    enableThinking: boolean;
    enableStreaming: boolean;
    selectedModel: SelectedModel;
}
export interface SelectedModel {
    providerName: string;
    model: string;
}

const defaultChatBehaviorSettings: ChatBehaviorSettings = {
    enableThinking: true,
    enableStreaming: true,
    selectedModel: {
        providerName: 'deepseek',
        model: 'deepseek-chat'
    }
};

export const storage = {
    // User Storage
    getUser: (): API.LoginUserDto | null => {
        try {
            const storedUser = localStorage.getItem(STORAGE_KEY_USER);
            if (!storedUser) return null;
            const parsedUser = JSON.parse(storedUser);
            return parsedUser?.user || null;
        } catch (e) {
            localStorage.removeItem(STORAGE_KEY_USER);
            return null;
        }
    },

    setUser: (user: API.LoginUserDto | null): void => {
        if (user) {
            localStorage.setItem(STORAGE_KEY_USER, JSON.stringify({ user }));
            window.dispatchEvent(new CustomEvent('userStorageChanged', { detail: { user } }));
        } else {
            localStorage.removeItem(STORAGE_KEY_USER);
            window.dispatchEvent(new CustomEvent('userStorageChanged', { detail: { user: null } }));
        }
    },

    clearUser: (): void => {
        localStorage.removeItem(STORAGE_KEY_USER);
        window.dispatchEvent(new CustomEvent('userStorageChanged', { detail: { user: null } }));
    },

    // Layout Settings
    getLayoutSettings: (): LayoutSettings => {
        try {
            const stored = localStorage.getItem(STORAGE_KEY_LAYOUT);
            if (!stored) return defaultLayoutSettings;
            return JSON.parse(stored);
        } catch (e) {
            localStorage.removeItem(STORAGE_KEY_LAYOUT);
            return defaultLayoutSettings;
        }
    },

    setLayoutSettings: (settings: LayoutSettings): void => {
        localStorage.setItem(STORAGE_KEY_LAYOUT, JSON.stringify(settings));
        window.dispatchEvent(new CustomEvent('layoutSettingsChanged', { detail: settings }));
    },

    // Chat Display Settings
    getChatDisplaySettings: (): ChatDisplaySettings => {
        try {
            const stored = localStorage.getItem(STORAGE_KEY_CHAT_DISPLAY);
            if (!stored) return defaultChatDisplaySettings;
            return JSON.parse(stored);
        } catch (e) {
            localStorage.removeItem(STORAGE_KEY_CHAT_DISPLAY);
            return defaultChatDisplaySettings;
        }
    },

    setChatDisplaySettings: (settings: ChatDisplaySettings): void => {
        localStorage.setItem(STORAGE_KEY_CHAT_DISPLAY, JSON.stringify(settings));
        window.dispatchEvent(new CustomEvent('chatDisplaySettingsChanged', { detail: settings }));
    },

    // Chat Behavior Settings
    getChatBehaviorSettings: (): ChatBehaviorSettings => {
        try {
            if (typeof window !== 'undefined' && window.localStorage) {
                const stored = localStorage.getItem(STORAGE_KEY_CHAT_BEHAVIOR);
                if (!stored) return defaultChatBehaviorSettings;
                return JSON.parse(stored);
            }
            return defaultChatBehaviorSettings;
        } catch (e) {
            localStorage.removeItem(STORAGE_KEY_CHAT_BEHAVIOR);
            return defaultChatBehaviorSettings;
        }
    },

    setChatBehaviorSettings: (settings: ChatBehaviorSettings): void => {
        localStorage.setItem(STORAGE_KEY_CHAT_BEHAVIOR, JSON.stringify(settings));
        window.dispatchEvent(new CustomEvent('chatBehaviorSettingsChanged', { detail: settings }));
    },

    // Chat Providers
    getChatProviders: (): API.ChatProvider[] | null => {
        try {
            const stored = localStorage.getItem(STORAGE_KEY_CHAT_PROVIDERS);
            if (!stored) return null;
            const parsedProviders = JSON.parse(stored);
            return Array.isArray(parsedProviders) ? parsedProviders : null;
        } catch (e) {
            localStorage.removeItem(STORAGE_KEY_CHAT_PROVIDERS);
            return null;
        }
    },

    setChatProviders: (providers: API.ChatProvider[]): void => {
        if (!Array.isArray(providers)) {
            console.error("Cannot store providers: not an array");
            return;
        }
        localStorage.setItem(STORAGE_KEY_CHAT_PROVIDERS, JSON.stringify(providers));
    },

    // Set selected model shorthand method
    setSelectedModel: (selectedModel: SelectedModel): void => {
        if (!selectedModel) return;

        const currentSettings = storage.getChatBehaviorSettings();
        const updatedSettings = {
            ...currentSettings,
            selectedModel: selectedModel
        };
        storage.setChatBehaviorSettings(updatedSettings);
        window.dispatchEvent(new CustomEvent('selectedModelChanged', { detail: selectedModel }));
    },

    clearAllSettings: (): void => {
        localStorage.removeItem(STORAGE_KEY_LAYOUT);
        localStorage.removeItem(STORAGE_KEY_CHAT_DISPLAY);
        localStorage.removeItem(STORAGE_KEY_CHAT_BEHAVIOR);
        localStorage.removeItem(STORAGE_KEY_CHAT_PROVIDERS);

        window.dispatchEvent(new CustomEvent('layoutSettingsChanged', { detail: defaultLayoutSettings }));
        window.dispatchEvent(new CustomEvent('chatDisplaySettingsChanged', { detail: defaultChatDisplaySettings }));
        window.dispatchEvent(new CustomEvent('chatBehaviorSettingsChanged', { detail: defaultChatBehaviorSettings }));
        window.dispatchEvent(new CustomEvent('selectedModelChanged', { detail: defaultChatBehaviorSettings.selectedModel }));
    }
};

// For TypeScript type checking of CustomEvents
declare global {
    interface WindowEventMap {
        'userStorageChanged': CustomEvent<{ user: API.LoginUserDto | null }>;
        'layoutSettingsChanged': CustomEvent<LayoutSettings>;
        'chatDisplaySettingsChanged': CustomEvent<ChatDisplaySettings>;
        'chatBehaviorSettingsChanged': CustomEvent<ChatBehaviorSettings>;
        'selectedModelChanged': CustomEvent<SelectedModel>;
    }
} 