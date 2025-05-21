import { useCallback } from 'react';
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

    const updateDisplaySettings = useCallback((newSettings: ChatDisplaySettings) => {
        dispatch(setChatDisplaySettings(newSettings));
    }, [dispatch]);

    const updateBehaviorSettings = useCallback((newSettings: ChatBehaviorSettings) => {
        dispatch(setChatBehaviorSettings(newSettings));
    }, [dispatch]);

    const updateSelectedModel = useCallback((model: SelectedModel) => {
        dispatch(setSelectedModel(model));
    }, [dispatch]);

    const toggleDisplaySetting = useCallback((key: keyof ChatDisplaySettings) => {
        dispatch(toggleChatDisplaySetting(key));
    }, [dispatch]);

    const toggleBehaviorSetting = useCallback((key: keyof Omit<ChatBehaviorSettings, 'selectedModel'>) => {
        dispatch(toggleChatBehaviorSetting(key));
    }, [dispatch]);

    const updateLayoutSettings = useCallback((newSettings: LayoutSettings) => {
        dispatch(setLayoutSettings(newSettings));
    }, [dispatch]);

    const loadProviders = useCallback(async () => {
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
    }, [dispatch, settings.chatProviders]);

    const handleResetSettings = useCallback(() => {
        dispatch(resetSettings());
    }, [dispatch]);

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