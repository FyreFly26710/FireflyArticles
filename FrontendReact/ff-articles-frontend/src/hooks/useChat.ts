import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
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
    updateLastChatRound,
    updateSessionRounds
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
    const chatState = useSelector((state: RootState) => state.chat);
    const settings = useSelector((state: RootState) => state.settings);

    const initialize = async () => {
        if (!chatState.sessions.length) {
            dispatch(setLoading(true));
            dispatch(setBehaviorSettings(settings.chatBehavior));

            const response = await apiSessionGetSessions({ includeChatRounds: true });
            const sessions = response.data || [];

            if (sessions.length > 0) {
                dispatch(setSessions(sessions));
                dispatch(setSession(sessions[0]));
            } else {
                const newSession: API.SessionDto = {
                    sessionId: 0,
                    sessionName: 'New Chat',
                    rounds: [],
                    roundCount: 0,
                    timestamp: Date.now(),
                    createTime: new Date().toISOString(),
                    updateTime: new Date().toISOString()
                };
                dispatch(setSession(newSession));
                dispatch(setSessions([newSession]));
            }

            await getProviders();
            dispatch(setLoading(false));
        }
    };

    const handleSendMessage = async (messageRequest: API.ChatRoundCreateRequest) => {
        const request = {
            ...messageRequest,
            model: chatState.behaviorSettings.selectedModel.model,
            provider: chatState.behaviorSettings.selectedModel.providerName
        };

        dispatch(setIsGenerating(true));

        const placeholderRound: API.ChatRoundDto = {
            sessionId: request.sessionId,
            chatRoundId: Date.now(),
            userMessage: request.userMessage,
            assistantMessage: "",
            model: request.model,
            provider: request.provider,
            createTime: new Date().toISOString(),
            updateTime: new Date().toISOString(),
            promptTokens: 0,
            completionTokens: 0,
            totalTokens: 0,
            timeTaken: 0,
            isActive: true
        };

        // Initialize session with placeholder round
        const updatedRounds = [...(chatState.session.rounds || []), placeholderRound];
        dispatch(updateSessionRounds({
            rounds: updatedRounds,
            roundCount: updatedRounds.length
        }));

        let messageBuffer = "";

        if (chatState.behaviorSettings.enableStreaming) {
            await apiChatRoundStreamResponse(request, {
                onInit: (data) => {
                    dispatch(updateLastChatRound({
                        chatRoundId: data.chatRoundId,
                        sessionId: data.sessionId
                    }));
                },
                onChunk: (content) => {
                    messageBuffer += content;
                    dispatch(updateLastChatRound({
                        assistantMessage: messageBuffer
                    }));
                },
                onDone: (data) => {
                    dispatch(updateLastChatRound({
                        ...data,
                        promptTokens: data.promptTokens || 0,
                        completionTokens: data.completionTokens || 0,
                        timeTaken: data.timeTaken || 0,
                        isActive: true
                    }));
                    dispatch(setIsGenerating(false));
                },
                onError: (data: Error) => {
                    dispatch(updateLastChatRound({
                        assistantMessage: data.message
                    }));
                    dispatch(setIsGenerating(false));
                }
            });
        } else {
            const response = await apiChatRoundAddByRequest(request);
            if (response.data) {
                dispatch(updateLastChatRound(response.data));
            }
            dispatch(setIsGenerating(false));
        }
    };

    const handleSelectSession = async (session: API.SessionDto) => {
        dispatch(setLoading(true));
        const response = await apiSessionGetById({ id: session.sessionId });
        if (response.data) {
            dispatch(setSession(response.data));
        }
        dispatch(setLoading(false));
    };

    const handleEditSessionName = async (session: API.SessionDto) => {
        await apiSessionUpdate({
            sessionId: session.sessionId,
            sessionName: session.sessionName
        });
        dispatch(setSessions(
            chatState.sessions.map(s => s.sessionId === session.sessionId ? session : s)
        ));
    };

    const handleDeleteSession = async (session: API.SessionDto) => {
        await apiSessionDeleteById({ id: session.sessionId });
        const updatedSessions = chatState.sessions.filter(s => s.sessionId !== session.sessionId);
        dispatch(setSessions(updatedSessions));

        if (chatState.session.sessionId === session.sessionId) {
            if (updatedSessions.length > 0) {
                dispatch(setSession(updatedSessions[0]));
            } else {
                const newSession: API.SessionDto = {
                    sessionId: 0,
                    sessionName: 'New Chat',
                    rounds: [],
                    roundCount: 0,
                    timestamp: Date.now(),
                    createTime: new Date().toISOString(),
                    updateTime: new Date().toISOString()
                };
                dispatch(setSession(newSession));
                dispatch(setSessions([newSession]));
            }
        }
    };

    const disableChatRounds = async (chatRoundIds: number[]) => {
        await apiChatRoundDisable(chatRoundIds);
        const updatedRounds = chatState.session.rounds?.map(round =>
            chatRoundIds.includes(round.chatRoundId) ? { ...round, isActive: false } : round
        ) || [];
        dispatch(updateSessionRounds({ rounds: updatedRounds, roundCount: updatedRounds.length }));
    };

    const enableChatRounds = async (chatRoundIds: number[]) => {
        await apiChatRoundEnable(chatRoundIds);
        const updatedRounds = chatState.session.rounds?.map(round =>
            chatRoundIds.includes(round.chatRoundId) ? { ...round, isActive: true } : round
        ) || [];
        dispatch(updateSessionRounds({ rounds: updatedRounds, roundCount: updatedRounds.length }));
    };

    const deleteChatRounds = async (chatRoundIds: number[]) => {
        await apiChatRoundDeleteByIds(chatRoundIds);
        const updatedRounds = chatState.session.rounds?.filter(
            round => !chatRoundIds.includes(round.chatRoundId)
        ) || [];
        dispatch(updateSessionRounds({ rounds: updatedRounds, roundCount: updatedRounds.length }));
    };

    const refreshCurrentSession = async () => {
        if (!chatState.session.sessionId) return;
        const response = await apiSessionGetById({ id: chatState.session.sessionId });
        const sessionData = response.data;
        if (sessionData) {
            dispatch(setSession(sessionData));
            const updatedSessions = chatState.sessions.map(s =>
                s.sessionId === sessionData.sessionId ? sessionData : s
            );
            dispatch(setSessions(updatedSessions));
        }
    };

    const getProviders = async () => {
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
    };

    const selectModel = (model: SelectedModel) => {
        if (!model) return;
        dispatch(setSelectedModel(model));
        dispatch(setBehaviorSettings({
            ...settings.chatBehavior,
            selectedModel: model
        }));
    };

    const handleCreateSession = async () => {
        const newSession: API.SessionDto = {
            sessionId: 0,
            sessionName: 'New Chat',
            rounds: [],
            roundCount: 0,
            timestamp: Date.now(),
            createTime: new Date().toISOString(),
            updateTime: new Date().toISOString()
        };
        dispatch(setSession(newSession));
        dispatch(setSessions([...chatState.sessions, newSession]));
    };

    return {
        session: chatState.session,
        sessions: chatState.sessions,
        loading: chatState.loading,
        isGenerating: chatState.isGenerating,
        providers: chatState.providers,
        behaviorSettings: chatState.behaviorSettings,
        initialize,
        handleSendMessage,
        handleCreateSession,
        handleSelectSession,
        handleEditSessionName,
        handleDeleteSession,
        disableChatRounds,
        enableChatRounds,
        deleteChatRounds,
        refreshCurrentSession,
        getProviders,
        selectModel,
    };
}; 