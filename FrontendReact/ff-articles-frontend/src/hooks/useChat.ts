import { useCallback, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { message } from 'antd';
import { RootState } from '@/stores';
import { SelectedModel } from '@/types';
import {
    setSession,
    setSessions,
    setLoading,
    setIsGenerating,
    setProviders,
    setBehaviorSettings,
    addChatRound,
    updateLastChatRound
} from '@/stores/chatSlice';
import { setChatProviders, setSelectedModel } from '@/stores/settingsSlice';
import {
    apiSessionGetSessions,
    apiSessionGetById,
    apiSessionUpdate,
    apiSessionDeleteById
} from '@/api/ai/api/session';
import {
    apiChatRoundAddByRequest,
    apiChatRoundDeleteById,
    apiChatRoundDisable,
    apiChatRoundEnable,
    apiChatRoundDeleteByIds,
    apiChatRoundStreamResponse
} from '@/api/ai/api/chatround';
import { apiAiAssistantProviders } from '@/api/ai/api/assistant';

export const useChat = () => {
    const dispatch = useDispatch();
    const {
        session,
        sessions,
        loading,
        isGenerating,
        providers,
        behaviorSettings
    } = useSelector((state: RootState) => state.chat);

    const settings = useSelector((state: RootState) => state.settings);

    // Initialize chat behavior settings from Redux settings
    useEffect(() => {
        dispatch(setBehaviorSettings(settings.chatBehavior));
    }, [dispatch, settings.chatBehavior]);

    // Load sessions on mount
    useEffect(() => {
        const fetchSessions = async () => {
            try {
                dispatch(setLoading(true));
                const response = await apiSessionGetSessions({ includeChatRounds: false });
                const sessionData = Array.isArray(response.data) ? response.data : [];

                const validSessions = sessionData.map(session => ({
                    ...session,
                    rounds: Array.isArray(session.rounds) ? session.rounds : []
                }));

                dispatch(setSessions(validSessions));

                if (validSessions.length === 0) {
                    handleCreateSession();
                    return;
                }

                const firstSession = validSessions[0];
                if (firstSession.sessionId > 0) {
                    try {
                        const sessionResponse = await apiSessionGetById({ id: firstSession.sessionId });
                        if (sessionResponse.data) {
                            dispatch(setSession({
                                ...sessionResponse.data,
                                rounds: Array.isArray(sessionResponse.data.rounds) ? sessionResponse.data.rounds : []
                            }));
                        } else {
                            dispatch(setSession(firstSession));
                        }
                    } catch (err) {
                        console.error('Error fetching session details:', err);
                        dispatch(setSession(firstSession));
                    }
                } else {
                    dispatch(setSession(firstSession));
                }
            } catch (error) {
                console.error('Error fetching sessions:', error);
                handleCreateSession();
            } finally {
                dispatch(setLoading(false));
            }
        };

        fetchSessions();
    }, [dispatch]);

    const handleSendMessage = useCallback(async (messageRequest: API.ChatRoundCreateRequest) => {
        const enrichedRequest = {
            ...messageRequest,
            model: behaviorSettings.selectedModel.model,
            provider: behaviorSettings.selectedModel.providerName
        };

        try {
            dispatch(setIsGenerating(true));

            const placeholderRound: API.ChatRoundDto = {
                sessionId: enrichedRequest.sessionId,
                chatRoundId: Date.now(),
                userMessage: enrichedRequest.userMessage,
                assistantMessage: "",
                model: enrichedRequest.model,
                provider: enrichedRequest.provider,
                createTime: new Date().toISOString(),
                updateTime: new Date().toISOString(),
                promptTokens: 0,
                completionTokens: 0,
                totalTokens: 0,
                timeTaken: 0,
                isActive: true
            };

            dispatch(addChatRound(placeholderRound));

            if (behaviorSettings.enableStreaming) {
                await apiChatRoundStreamResponse(
                    enrichedRequest,
                    {
                        onInit: (data) => {
                            dispatch(updateLastChatRound({
                                chatRoundId: data.chatRoundId,
                                sessionId: data.sessionId
                            }));
                        },
                        onChunk: (content) => {
                            dispatch(updateLastChatRound({
                                assistantMessage: (session.rounds?.[session.rounds.length - 1]?.assistantMessage || '') + content
                            }));
                        },
                        onDone: (data) => {
                            dispatch(setIsGenerating(false));
                            dispatch(updateLastChatRound({
                                ...data,
                                promptTokens: data.promptTokens || 0,
                                completionTokens: data.completionTokens || 0,
                                timeTaken: data.timeTaken || 0,
                                isActive: true
                            }));
                        },
                        onError: (error: Error) => {
                            dispatch(setIsGenerating(false));
                            dispatch(updateLastChatRound({
                                assistantMessage: `Error: ${error.message}`,
                                isActive: true
                            }));
                        }
                    }
                );
            } else {
                const response = await apiChatRoundAddByRequest(enrichedRequest);
                if (response.data) {
                    dispatch(updateLastChatRound(response.data));
                }
                dispatch(setIsGenerating(false));
            }
        } catch (error) {
            dispatch(setIsGenerating(false));
            console.error('Error sending message:', error);
            message.error('Failed to send message');
        }
    }, [dispatch, session, behaviorSettings]);

    const handleCreateSession = useCallback(async () => {
        const newSession: API.SessionDto = {
            sessionId: 0,
            sessionName: 'New Chat',
            rounds: [],
            roundCount: 0,
            timestamp: Date.now(),
            createTime: new Date().toISOString(),
            updateTime: new Date().toISOString()
        };

        dispatch(setSessions([...sessions, newSession]));
        dispatch(setSession(newSession));
    }, [dispatch, sessions]);

    const handleSelectSession = useCallback(async (selectedSession: API.SessionDto) => {
        try {
            dispatch(setLoading(true));
            const response = await apiSessionGetById({ id: selectedSession.sessionId });
            dispatch(setSession(response.data || selectedSession));
        } catch (error) {
            console.error('Error selecting session:', error);
            dispatch(setSession(selectedSession));
        } finally {
            dispatch(setLoading(false));
        }
    }, [dispatch]);

    const handleEditSessionName = useCallback(async (updatedSession: API.SessionDto) => {
        try {
            await apiSessionUpdate({
                sessionId: updatedSession.sessionId,
                sessionName: updatedSession.sessionName
            });

            dispatch(setSessions(
                sessions.map(s => s.sessionId === updatedSession.sessionId ? updatedSession : s)
            ));
        } catch (error) {
            console.error('Error updating session name:', error);
            message.error('Failed to update session name');
        }
    }, [dispatch, sessions]);

    const handleDeleteSession = useCallback(async (sessionToDelete: API.SessionDto) => {
        try {
            await apiSessionDeleteById({ id: sessionToDelete.sessionId });
            
            const updatedSessions = sessions.filter(s => s.sessionId !== sessionToDelete.sessionId);
            dispatch(setSessions(updatedSessions));

            if (session.sessionId === sessionToDelete.sessionId) {
                if (updatedSessions.length > 0) {
                    dispatch(setSession(updatedSessions[0]));
                } else {
                    handleCreateSession();
                }
            }
        } catch (error) {
            console.error('Error deleting session:', error);
            message.error('Failed to delete session');
        }
    }, [dispatch, sessions, session, handleCreateSession]);

    const handleDisableChatRounds = useCallback(async (chatRoundIds: number[]) => {
        try {
            await apiChatRoundDisable(chatRoundIds);
            await refreshCurrentSession();
        } catch (error) {
            console.error('Error disabling chat rounds:', error);
            message.error('Failed to disable chat rounds');
        }
    }, []);

    const handleEnableChatRounds = useCallback(async (chatRoundIds: number[]) => {
        try {
            await apiChatRoundEnable(chatRoundIds);
            await refreshCurrentSession();
        } catch (error) {
            console.error('Error enabling chat rounds:', error);
            message.error('Failed to enable chat rounds');
        }
    }, []);

    const handleDeleteChatRounds = useCallback(async (chatRoundIds: number[]) => {
        try {
            await apiChatRoundDeleteByIds(chatRoundIds);
            await refreshCurrentSession();
        } catch (error) {
            console.error('Error deleting chat rounds:', error);
            message.error('Failed to delete chat rounds');
        }
    }, []);

    const refreshCurrentSession = useCallback(async () => {
        try {
            if (!session.sessionId || session.sessionId <= 0) return;

            const response = await apiSessionGetById({ id: session.sessionId });
            if (response.data) {
                const updatedSession = {
                    ...response.data,
                    rounds: Array.isArray(response.data.rounds) ? response.data.rounds : []
                };
                dispatch(setSession(updatedSession));
                dispatch(setSessions(
                    sessions.map(s => s.sessionId === updatedSession.sessionId ? updatedSession : s)
                ));
            }
        } catch (error) {
            console.error('Error refreshing session:', error);
            message.error('Failed to refresh session');
        }
    }, [dispatch, session, sessions]);

    const getProviders = useCallback(async () => {
        try {
            if (settings.chatProviders) {
                dispatch(setProviders(settings.chatProviders));
                return settings.chatProviders;
            }

            const response = await apiAiAssistantProviders();
            if (response.data) {
                dispatch(setProviders(response.data));
                dispatch(setChatProviders(response.data));
                return response.data;
            }
            return [];
        } catch (error) {
            console.error('Error fetching providers:', error);
            return [];
        }
    }, [dispatch, settings.chatProviders]);

    const selectModel = useCallback((model: SelectedModel) => {
        if (!model) return;
        dispatch(setSelectedModel(model));
        dispatch(setBehaviorSettings({
            ...settings.chatBehavior,
            selectedModel: model
        }));
    }, [dispatch, settings.chatBehavior]);

    return {
        // State
        session,
        sessions,
        loading,
        isGenerating,
        providers,
        behaviorSettings,

        // Actions
        handleSendMessage,
        handleCreateSession,
        handleSelectSession,
        handleEditSessionName,
        handleDeleteSession,
        handleDisableChatRounds,
        handleEnableChatRounds,
        handleDeleteChatRounds,
        refreshCurrentSession,
        getProviders,
        selectModel,
    };
}; 