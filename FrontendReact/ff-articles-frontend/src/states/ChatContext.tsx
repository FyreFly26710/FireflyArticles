'use client';

import { createContext, useContext, useState, ReactNode, useEffect } from 'react';
import { apiSessionGetSessions, apiSessionGetById, apiSessionUpdate, apiSessionDeleteById } from '@/api/ai/api/session';
import {
  apiChatRoundAddByRequest,
  apiChatRoundDeleteById,
  apiChatRoundDisable,
  apiChatRoundEnable,
  apiChatRoundDeleteByIds,
  apiChatRoundStreamResponse
} from '@/api/ai/api/chatround';
import { apiAiAssistantProviders } from '@/api/ai/api/assistant';
import { ChatBehaviorSettings, SelectedModel, storage } from './storage';

interface ChatContextType {
  session: API.SessionDto;
  sessions: API.SessionDto[];
  loading: boolean;
  isGenerating: boolean;
  providers: API.ChatProvider[];
  behaviorSettings: ChatBehaviorSettings;
  // Setters
  setSession: (session: API.SessionDto) => void;
  setSessions: (sessions: API.SessionDto[]) => void;
  // Handlers
  handleSendMessage: (message: API.ChatRoundCreateRequest) => Promise<void>;
  handleCreateSession: () => Promise<void>;
  handleSelectSession: (selectedSession: API.SessionDto) => Promise<void>;
  handleEditSessionName: (updatedSession: API.SessionDto) => Promise<void>;
  handleDeleteSession: (sessionToDelete: API.SessionDto) => Promise<void>;
  handleDisableChatRounds: (chatRoundIds: number[]) => Promise<void>;
  handleEnableChatRounds: (chatRoundIds: number[]) => Promise<void>;
  handleDeleteChatRounds: (chatRoundIds: number[]) => Promise<void>;
  refreshCurrentSession: () => Promise<void>;
  getProviders: () => Promise<API.ChatProvider[]>;
  selectModel: (model: SelectedModel) => void;
}

const ChatContext = createContext<ChatContextType | undefined>(undefined);

// Helper for creating empty session
const createEmptySession = (): API.SessionDto => ({
  sessionId: 0,
  sessionName: 'New Chat',
  rounds: [],
  roundCount: 0,
  createTime: new Date().toISOString(),
  updateTime: new Date().toISOString(),
  timestamp: Date.now()
});


export function ChatProvider({ children }: { children: ReactNode }) {

  const settings = storage.getChatBehaviorSettings();

  const [session, setSession] = useState<API.SessionDto>(createEmptySession());
  const [sessions, setSessions] = useState<API.SessionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [isGenerating, setIsGenerating] = useState(false);
  const [providers, setProviders] = useState<API.ChatProvider[]>([]);
  const [behaviorSettings, setBehaviorSettings] = useState<ChatBehaviorSettings>(settings);

  useEffect(() => {
    const handleBehaviorChange = (event: CustomEvent<ChatBehaviorSettings>) => {
      setBehaviorSettings(event.detail);
    };
    window.addEventListener('chatBehaviorSettingsChanged', handleBehaviorChange);
    return () => {
      window.removeEventListener('chatBehaviorSettingsChanged', handleBehaviorChange);
    };
  }, []);

  // // Load providers on component mount
  // useEffect(() => {
  //   getProviders();
  // }, []);

  // Load sessions on component mount
  useEffect(() => {
    const fetchSessions = async () => {
      try {
        setLoading(true);
        const response = await apiSessionGetSessions({ includeChatRounds: false });

        // Ensure we have a valid array of sessions
        const sessionData = Array.isArray(response.data) ? response.data : [];
        console.log('Fetched sessions:', sessionData);

        // Validate each session has a proper rounds array
        const validSessions = sessionData.map(session => {
          if (!session.rounds || !Array.isArray(session.rounds)) {
            return { ...session, rounds: [] };
          }
          return session;
        });

        setSessions(validSessions);

        // Early return if no sessions exist
        if (validSessions.length === 0) {
          handleCreateSession();
          return;
        }

        // Process the first session
        const firstSession = validSessions[0];
        if (firstSession.sessionId > 0) {
          try {
            const sessionResponse = await apiSessionGetById({ id: firstSession.sessionId });
            // Ensure response data has proper rounds array
            if (sessionResponse.data) {
              if (!sessionResponse.data.rounds || !Array.isArray(sessionResponse.data.rounds)) {
                sessionResponse.data.rounds = [];
              }
              setSession(sessionResponse.data);
            } else {
              // Ensure firstSession has proper rounds array
              setSession(firstSession);
            }
          } catch (err) {
            console.error('Error fetching session details:', err);
            setSession(firstSession);
          }
        } else {
          setSession(firstSession);
        }

      } catch (error) {
        console.error('Error fetching sessions:', error);
        // Initialize with default session if API fails
        handleCreateSession();
      } finally {
        setLoading(false);
      }
    };

    fetchSessions();
  }, []);

  const handleSendMessage = async (messageRequest: API.ChatRoundCreateRequest) => {
    console.log('behaviorSettings', behaviorSettings);
    messageRequest = {
      ...messageRequest,
      model: behaviorSettings.selectedModel.model,
      provider: behaviorSettings.selectedModel.providerName
    };
    console.log('handleSendMessage', messageRequest);

    try {
      setIsGenerating(true);  // Start generating
      // Create a placeholder response for UI feedback with proper defaults
      const placeholderChatRound: API.ChatRoundDto = {
        sessionId: messageRequest.sessionId,
        chatRoundId: Date.now(),
        userMessage: messageRequest.userMessage,
        assistantMessage: "",
        model: messageRequest.model || behaviorSettings.selectedModel.model,
        provider: messageRequest.provider || behaviorSettings.selectedModel.providerName,
        createTime: new Date().toISOString(),
        updateTime: new Date().toISOString(),
        promptTokens: 0,
        completionTokens: 0,
        totalTokens: 0,
        timeTaken: 0,
        isActive: true  // Start as active
      };

      // Add the message to the UI
      setSession(prev => {
        const updatedSession = { ...prev };
        // Ensure rounds is always an array
        if (!updatedSession.rounds || !Array.isArray(updatedSession.rounds)) {
          updatedSession.rounds = [];
        }
        updatedSession.rounds = [...updatedSession.rounds, placeholderChatRound];
        updatedSession.roundCount = updatedSession.rounds.length;
        return updatedSession;
      });

      if (behaviorSettings.enableStreaming) {
        await apiChatRoundStreamResponse(
          messageRequest,
          {
            onInit: (data) => {
              setSession(prev => {
                const updatedSession = { ...prev };
                // Ensure rounds is always an array
                if (!updatedSession.rounds || !Array.isArray(updatedSession.rounds)) {
                  updatedSession.rounds = [];
                }
                const updatedRounds = [...updatedSession.rounds];

                if (updatedRounds.length > 0) {
                  const lastRound = { ...updatedRounds[updatedRounds.length - 1] };
                  lastRound.chatRoundId = data.chatRoundId;
                  lastRound.sessionId = data.sessionId;
                  // lastRound.isActive = true; 
                  updatedRounds[updatedRounds.length - 1] = lastRound;
                }

                updatedSession.rounds = updatedRounds;
                return updatedSession;
              });
            },
            onChunk: (content) => {
              setSession(prev => {
                const updatedSession = { ...prev };
                // Ensure rounds is always an array
                if (!updatedSession.rounds || !Array.isArray(updatedSession.rounds)) {
                  updatedSession.rounds = [];
                }
                const updatedRounds = [...updatedSession.rounds];

                if (updatedRounds.length > 0) {
                  const lastRound = { ...updatedRounds[updatedRounds.length - 1] };
                  lastRound.assistantMessage = (lastRound.assistantMessage || '') + content;
                  updatedRounds[updatedRounds.length - 1] = lastRound;
                }

                updatedSession.rounds = updatedRounds;
                return updatedSession;
              });
            },
            onDone: (data) => {
              setIsGenerating(false);  // Stop generating
              data = {
                ...data,
                promptTokens: data.promptTokens || 0,
                completionTokens: data.completionTokens || 0,
                timeTaken: data.timeTaken || 0,
              };
              updateSessionsByChatRound(data);
            },
            onError: (error: Error) => {
              setIsGenerating(false);  // Stop generating on error
              setSession(prev => {
                const updatedSession = { ...prev };
                // Ensure rounds is always an array
                if (!updatedSession.rounds || !Array.isArray(updatedSession.rounds)) {
                  updatedSession.rounds = [];
                }
                const updatedRounds = [...updatedSession.rounds];

                if (updatedRounds.length > 0) {
                  const lastRound = { ...updatedRounds[updatedRounds.length - 1] };
                  lastRound.assistantMessage = (lastRound.assistantMessage || '') +
                    "\n\nSorry, there was an error generating the rest of the response: \n\n" + error.message;
                  lastRound.isActive = true; // Keep message active even on error
                  updatedRounds[updatedRounds.length - 1] = lastRound;
                }

                updatedSession.rounds = updatedRounds;
                return updatedSession;
              });
            }
          }
        );
      } else {
        // Non-streaming chat
        const response = await apiChatRoundAddByRequest(messageRequest);
        if (!response.data) {
          throw new Error('No data returned from API');
        }
        updateSessionsByChatRound(response.data);
        setIsGenerating(false);
      }
    } catch (error) {
      setIsGenerating(false);
      console.error('Error getting assistant response:', error);
      setSession(prev => {
        const updatedSession = { ...prev };
        // Ensure rounds is always an array
        if (!updatedSession.rounds || !Array.isArray(updatedSession.rounds)) {
          updatedSession.rounds = [];
        }
        const updatedRounds = [...updatedSession.rounds];

        if (updatedRounds.length > 0) {
          const lastRound = { ...updatedRounds[updatedRounds.length - 1] };
          lastRound.assistantMessage = "Sorry, there was an error generating a response.";
          updatedRounds[updatedRounds.length - 1] = lastRound;
        }

        updatedSession.rounds = updatedRounds;
        return updatedSession;
      });
    }
  };

  const updateSessionsByChatRound = (updatedRound: API.ChatRoundDto) => {
    // Update with the actual response
    setSession(prev => {
      const updatedSession = { ...prev };
      // Ensure rounds is always an array
      if (!updatedSession.rounds || !Array.isArray(updatedSession.rounds)) {
        updatedSession.rounds = [];
      }
      const updatedRounds = [...updatedSession.rounds];

      // Replace the placeholder with the actual response
      if (updatedRounds.length > 0 && updatedRound) {
        const lastRound = updatedRounds[updatedRounds.length - 1];
        // Create merged data preserving all metadata
        const mergedData = {
          ...lastRound,  // Keep existing metadata
          ...updatedRound,  // Override with new data
          // Ensure these fields are properly set
          assistantMessage: updatedRound.assistantMessage || lastRound.assistantMessage,
          promptTokens: updatedRound.promptTokens ?? lastRound.promptTokens ?? 0,
          completionTokens: updatedRound.completionTokens ?? lastRound.completionTokens ?? 0,
          totalTokens: updatedRound.totalTokens ?? (updatedRound.promptTokens + updatedRound.completionTokens) ?? lastRound.totalTokens ?? 0,
          timeTaken: updatedRound.timeTaken ?? lastRound.timeTaken ?? 0,
          isActive: true,
          updateTime: new Date().toISOString()
        };

        updatedRounds[updatedRounds.length - 1] = mergedData;
      } else if (updatedRound) {
        updatedRounds.push(updatedRound);
      }

      // Update session ID if API created a new session
      const newSessionId = updatedRound.sessionId;
      const sessionIdChanged = updatedSession.sessionId !== newSessionId;
      updatedSession.sessionId = newSessionId;
      updatedSession.rounds = updatedRounds;
      updatedSession.roundCount = updatedRounds.length;

      // If session ID changed, update the sessions array too
      if (sessionIdChanged) {
        setSessions(prevSessions => {
          const filteredSessions = prevSessions.filter(s => s.sessionId !== prev.sessionId);
          const sessionIndex = filteredSessions.findIndex(s => s.sessionId === newSessionId);

          if (sessionIndex >= 0) {
            const updatedSessions = [...filteredSessions];
            updatedSessions[sessionIndex] = updatedSession;
            return updatedSessions;
          } else {
            return [...filteredSessions, updatedSession];
          }
        });
      }

      return updatedSession;
    });
  };
  // Create a new session
  const handleCreateSession = async () => {
    try {
      // Default session name
      const name = 'New Chat';

      // Create a new session
      const newSession: API.SessionDto = {
        sessionId: 0,
        sessionName: name,
        rounds: [],
        roundCount: 0,
        timestamp: Date.now(),
        createTime: new Date().toISOString(),
        updateTime: new Date().toISOString()
      };

      // Ensure we're adding to a valid array
      setSessions(prev => {
        if (!Array.isArray(prev)) {
          console.error('sessions state is not an array in handleCreateSession:', prev);
          return [newSession];
        }
        return [...prev, newSession];
      });

      setSession(newSession);
    } catch (error) {
      console.error('Error creating new session:', error);
    }
  };

  const handleSelectSession = async (selectedSession: API.SessionDto) => {
    try {
      // Fetch the full session with all chat rounds from the API
      setLoading(true);
      const response = await apiSessionGetById({ id: selectedSession.sessionId });
      if (response.data) {
        setSession(response.data);
      } else {
        setSession(selectedSession);
      }
    } catch (error) {
      console.error('Error selecting session:', error);
      setSession(selectedSession);
    } finally {
      setLoading(false);
    }
  };
  // Only update existing session name
  const handleEditSessionName = async (updatedSession: API.SessionDto) => {
    try {
      // Update session name on the backend
      await apiSessionUpdate({
        sessionId: updatedSession.sessionId,
        sessionName: updatedSession.sessionName
      });

      // Update local state if successful with array validation
      setSessions(prev => {
        if (!Array.isArray(prev)) {
          console.error('sessions state is not an array in handleEditSessionName:', prev);
          return [updatedSession];
        }
        return prev.map(s =>
          s.sessionId === updatedSession.sessionId ? updatedSession : s
        );
      });

      // Update current session if it's the one being edited
      // if (session.sessionId === updatedSession.sessionId) {
      //   setSession(updatedSession);
      // }
    } catch (error) {
      console.error('Error updating session name:', error);
    }
  };

  const handleDeleteSession = async (sessionToDelete: API.SessionDto) => {
    try {
      // Delete session on the backend
      await apiSessionDeleteById({ id: sessionToDelete.sessionId });

      // Update local state if successful with array validation
      setSessions(prev => {
        if (!Array.isArray(prev)) {
          console.error('sessions state is not an array in handleDeleteSession:', prev);
          return [];
        }
        return prev.filter(s => s.sessionId !== sessionToDelete.sessionId);
      });

      if (session.sessionId === sessionToDelete.sessionId) {
        if (sessions.length > 1) {
          // Find another session to display
          const nextSession = sessions.find(s => s.sessionId !== sessionToDelete.sessionId);
          if (nextSession) {
            setSession(nextSession);
          }
        } else {
          // Create a new session if this was the last one
          handleCreateSession();
        }
      }
    } catch (error) {
      console.error('Error deleting session:', error);
    }
  };

  const handleDisableChatRounds = async (chatRoundIds: number[]) => {
    try {
      if (!chatRoundIds.length) return;

      await apiChatRoundDisable(chatRoundIds);
      // Refresh the current session to reflect changes
      await refreshCurrentSession();
    } catch (error) {
      console.error('Error disabling chat rounds:', error);
    }
  };

  const handleEnableChatRounds = async (chatRoundIds: number[]) => {
    try {
      if (!chatRoundIds.length) return;

      await apiChatRoundEnable(chatRoundIds);

      // Refresh the current session to reflect changes
      await refreshCurrentSession();
    } catch (error) {
      console.error('Error enabling chat rounds:', error);
    }
  };

  const handleDeleteChatRounds = async (chatRoundIds: number[]) => {
    try {
      if (!chatRoundIds.length) return;

      await apiChatRoundDeleteByIds(chatRoundIds);

      // Refresh the current session to reflect changes
      await refreshCurrentSession();
    } catch (error) {
      console.error('Error deleting chat rounds:', error);
    }
  };

  const refreshCurrentSession = async () => {
    try {
      if (!session.sessionId || session.sessionId <= 0) {
        console.log('Cannot refresh session with invalid sessionId:', session.sessionId);
        return;
      }

      const response = await apiSessionGetById({ id: session.sessionId });
      if (response.data) {
        // Ensure response.data has rounds as an array
        if (!response.data.rounds || !Array.isArray(response.data.rounds)) {
          response.data.rounds = [];
        }
        setSession(response.data);

        // Also update the session in the sessions list
        setSessions(prev => {
          // Make sure prev is an array before mapping
          if (!Array.isArray(prev)) {
            console.error('sessions state is not an array:', prev);
            return Array.isArray(response.data) ? [response.data] : [];
          }
          return prev.map(s => {
            if (s.sessionId === response.data?.sessionId) {
              return response.data;
            }
            return s;
          });
        });
      }
    } catch (error) {
      console.error('Error refreshing session:', error);
    }
  };

  // Get providers from storage or API
  const getProviders = async (): Promise<API.ChatProvider[]> => {
    try {
      // Check if we have providers in storage
      const storedProviders = storage.getChatProviders();
      if (storedProviders) {
        setProviders(storedProviders);
        return storedProviders;
      }

      // Fetch from API if not in storage
      const response = await apiAiAssistantProviders();

      // The response is already the ChatProviderListApiResponse
      if (response && response.data && Array.isArray(response.data)) {
        setProviders(response.data);
        // Store in localStorage for next time
        storage.setChatProviders(response.data);
        return response.data;
      }
      return [];
    } catch (error) {
      console.error('Error fetching providers:', error);
      return [];
    }
  };

  // Select a model and save to storage
  const selectModel = (model: SelectedModel) => {
    if (!model) return;

    setBehaviorSettings({ ...behaviorSettings, selectedModel: model });
    storage.setSelectedModel(model);
  };

  return (
    <ChatContext.Provider value={{
      session,
      sessions,
      loading,
      isGenerating,
      providers,
      behaviorSettings,
      // Setters
      setSession,
      setSessions,
      // Handlers
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
    }}>
      {children}
    </ChatContext.Provider>
  );
}

export function useChat() {
  const context = useContext(ChatContext);
  if (context === undefined) {
    throw new Error('useChat must be used within a ChatProvider');
  }
  return context;
} 