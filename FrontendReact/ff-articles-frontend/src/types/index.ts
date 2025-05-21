// 
// Settings
// 
export interface LayoutSettings {
    sidebarCollapsed: boolean;
    rightSidebarCollapsed: boolean;
}

export interface ChatDisplaySettings {
    showMessageTimestamp: boolean;
    showModelName: boolean;
    showTokenUsed: boolean;
    showTimeTaken: boolean;
    enableMarkdownRendering: boolean;
    showOnlyActiveMessages: boolean;
    enableCollapsibleMessages: boolean;
}

export interface SelectedModel {
    providerName: string;
    model: string;
}

export interface ChatBehaviorSettings {
    enableThinking: boolean;
    enableStreaming: boolean;
    selectedModel: SelectedModel;
}

export interface SettingsState {
    layout: LayoutSettings;
    chatDisplay: ChatDisplaySettings;
    chatBehavior: ChatBehaviorSettings;
    chatProviders: API.ChatProvider[] | null;
}

// 
// 
// 