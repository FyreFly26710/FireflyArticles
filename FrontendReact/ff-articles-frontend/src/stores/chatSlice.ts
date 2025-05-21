import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { ChatBehaviorSettings } from "@/types";

interface ChatState {
    session: API.SessionDto;
    sessions: API.SessionDto[];
    loading: boolean;
    isGenerating: boolean;
    providers: API.ChatProvider[];
    behaviorSettings: ChatBehaviorSettings;
}

const createEmptySession = (): API.SessionDto => ({
    sessionId: 0,
    sessionName: 'New Chat',
    rounds: [],
    roundCount: 0,
    createTime: new Date().toISOString(),
    updateTime: new Date().toISOString(),
    timestamp: Date.now()
});

const initialState: ChatState = {
    session: createEmptySession(),
    sessions: [],
    loading: false,
    isGenerating: false,
    providers: [],
    behaviorSettings: {
        enableThinking: true,
        enableStreaming: true,
        selectedModel: {
            providerName: 'deepseek',
            model: 'deepseek-chat'
        }
    }
};

export const chatSlice = createSlice({
    name: "chat",
    initialState,
    reducers: {
        setSession: (state, action: PayloadAction<API.SessionDto>) => {
            state.session = action.payload;
        },
        setSessions: (state, action: PayloadAction<API.SessionDto[]>) => {
            state.sessions = action.payload;
        },
        setLoading: (state, action: PayloadAction<boolean>) => {
            state.loading = action.payload;
        },
        setIsGenerating: (state, action: PayloadAction<boolean>) => {
            state.isGenerating = action.payload;
        },
        setProviders: (state, action: PayloadAction<API.ChatProvider[]>) => {
            state.providers = action.payload;
        },
        setBehaviorSettings: (state, action: PayloadAction<ChatBehaviorSettings>) => {
            state.behaviorSettings = action.payload;
        },
        updateSessionRounds: (state, action: PayloadAction<{
            rounds: API.ChatRoundDto[];
            roundCount: number;
        }>) => {
            state.session.rounds = action.payload.rounds;
            state.session.roundCount = action.payload.roundCount;
        },
        addChatRound: (state, action: PayloadAction<API.ChatRoundDto>) => {
            if (!state.session.rounds) {
                state.session.rounds = [];
            }
            state.session.rounds.push(action.payload);
            state.session.roundCount = state.session.rounds.length;
        },
        updateLastChatRound: (state, action: PayloadAction<Partial<API.ChatRoundDto>>) => {
            if (state.session.rounds && state.session.rounds.length > 0) {
                const lastIndex = state.session.rounds.length - 1;
                state.session.rounds[lastIndex] = {
                    ...state.session.rounds[lastIndex],
                    ...action.payload
                };
            }
        }
    }
});

export const {
    setSession,
    setSessions,
    setLoading,
    setIsGenerating,
    setProviders,
    setBehaviorSettings,
    updateSessionRounds,
    addChatRound,
    updateLastChatRound
} = chatSlice.actions; 