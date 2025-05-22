import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '@/stores';
import {
    setChatDisplaySettings,
    setChatBehaviorSettings,
    setSelectedModel,
    setChatProviders,
    toggleChatDisplaySetting,
    toggleChatBehaviorSetting,
    resetSettings,
    setLayoutSettings
} from '@/stores/settingsSlice';
import { ChatDisplaySettings, ChatBehaviorSettings, SelectedModel, LayoutSettings } from '@/types';
import { apiAiAssistantProviders } from '@/api/ai/api/assistant';

export const useSettings = () => {
    const dispatch = useDispatch();
    const settings = useSelector((state: RootState) => state.settings);

    const updateDisplaySettings = (newSettings: ChatDisplaySettings) => {
        dispatch(setChatDisplaySettings(newSettings));
    };

    const updateBehaviorSettings = (newSettings: ChatBehaviorSettings) => {
        dispatch(setChatBehaviorSettings(newSettings));
    };

    const updateSelectedModel = (model: SelectedModel) => {
        dispatch(setSelectedModel(model));
    };

    const toggleDisplaySetting = (key: keyof ChatDisplaySettings) => {
        dispatch(toggleChatDisplaySetting(key));
    };

    const toggleBehaviorSetting = (key: keyof Omit<ChatBehaviorSettings, 'selectedModel'>) => {
        dispatch(toggleChatBehaviorSetting(key));
    };

    const updateLayoutSettings = (newSettings: LayoutSettings) => {
        dispatch(setLayoutSettings(newSettings));
    };

    const loadProviders = async () => {
        try {
            if (settings.chatProviders) {
                return settings.chatProviders;
            }

            const response = await apiAiAssistantProviders();
            if (response?.data && Array.isArray(response.data)) {
                dispatch(setChatProviders(response.data));
                return response.data;
            }
            return [];
        } catch (error) {
            console.error('Error loading providers:', error);
            return [];
        }
    };

    const handleResetSettings = () => {
        dispatch(resetSettings());
    };

    return {
        // State
        settings,

        // Actions
        updateDisplaySettings,
        updateBehaviorSettings,
        updateSelectedModel,
        toggleDisplaySetting,
        toggleBehaviorSetting,
        updateLayoutSettings,
        loadProviders,
        handleResetSettings,
    };
}; 