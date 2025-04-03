const STORAGE_KEY_USER = 'user-storage';
const STORAGE_KEY_CHAT_SETTINGS = 'chat-settings';

export interface ChatSettings {
    showMessageTimestamp: boolean;
    showModelName: boolean;
    showTokenUsed: boolean;
    showTimeTaken: boolean;
    enableMarkdownRendering: boolean;
    enableThinking: boolean;
    enableStreaming: boolean;
    showOnlyActiveMessages: boolean;
    enableCollapsibleMessages: boolean;
    selectedModel: string;
}

export const storage = {
    getUser: (): API.LoginUserDto | null => {
        try {
            const storedUser = localStorage.getItem(STORAGE_KEY_USER);
            if (!storedUser) return null;

            const parsedUser = JSON.parse(storedUser);
            return parsedUser?.user || null;
        } catch (e) {
            // If there's any error, clear the storage and return null
            localStorage.removeItem(STORAGE_KEY_USER);
            return null;
        }
    },

    setUser: (user: API.LoginUserDto | null): void => {
        if (user) {
            localStorage.setItem(STORAGE_KEY_USER, JSON.stringify({ user }));
        } else {
            localStorage.removeItem(STORAGE_KEY_USER);
        }
    },

    clearUser: (): void => {
        localStorage.removeItem(STORAGE_KEY_USER);
    },

    getChatSettings: (): ChatSettings | null => {
        try {
            const storedSettings = localStorage.getItem(STORAGE_KEY_CHAT_SETTINGS);
            if (!storedSettings) return null;
            return JSON.parse(storedSettings);
        } catch (e) {
            localStorage.removeItem(STORAGE_KEY_CHAT_SETTINGS);
            return null;
        }
    },

    setChatSettings: (settings: ChatSettings): void => {
        localStorage.setItem(STORAGE_KEY_CHAT_SETTINGS, JSON.stringify(settings));
    },

    clearChatSettings: (): void => {
        localStorage.removeItem(STORAGE_KEY_CHAT_SETTINGS);
    }
}; 