'use client';

import { createContext, useContext, useState, ReactNode, useEffect } from 'react';
import { apiSessionGetSessions, apiSessionGetById, apiSessionUpdate, apiSessionDeleteById } from '@/api/ai/api/session';
import {
  apiChatRoundAddByRequest,
  apiChatRoundUpdateByRequest,
  apiChatRoundDeleteById,
  apiChatRoundDisable,
  apiChatRoundEnable,
  apiChatRoundDeleteByIds,
  apiChatRoundStreamResponse
} from '@/api/ai/api/chatround';
import { storage } from '@/stores/storage';

interface ChatContextType {
  session: API.SessionDto;
  sessions: API.SessionDto[];
  loading: boolean;
  isGenerating: boolean;
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
  const [session, setSession] = useState<API.SessionDto>(createEmptySession());
  const [sessions, setSessions] = useState<API.SessionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [isGenerating, setIsGenerating] = useState(false);

  // Get settings from storage
  const settings = storage.getChatBehaviorSettings();

  // Load sessions on component mount
  useEffect(() => {
    const fetchSessions = async () => {
      try {
        setLoading(true);
        const response: API.SessionDto[] = (await apiSessionGetSessions({ includeChatRounds: false })).data ?? [];
        setSessions(response);

        // Early return if no sessions exist
        if (response.length === 0) {
          handleCreateSession();
          return;
        }

        // Process the first session
        const firstSession = response[0];
        if (firstSession.sessionId > 0) {
          const sessionResponse = await apiSessionGetById({ id: firstSession.sessionId });
          setSession(sessionResponse.data || firstSession);
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
    try {
      setIsGenerating(true);  // Start generating
      // Create a placeholder response for UI feedback with proper defaults
      const placeholderChatRound: API.ChatRoundDto = {
        sessionId: messageRequest.sessionId,
        chatRoundId: Date.now(),
        userMessage: messageRequest.userMessage,
        assistantMessage: "",
        model: messageRequest.model || "deepseek",
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
        if (!updatedSession.rounds) {
          updatedSession.rounds = [];
        }
        updatedSession.rounds = [...updatedSession.rounds, placeholderChatRound];
        updatedSession.roundCount = updatedSession.rounds.length;
        return updatedSession;
      });

      if (settings.enableStreaming) {
        await apiChatRoundStreamResponse(
          messageRequest,
          {
            onInit: (data) => {
              setSession(prev => {
                const updatedSession = { ...prev };
                const updatedRounds = [...(updatedSession.rounds || [])];

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
                const updatedRounds = [...(updatedSession.rounds || [])];

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
            onError: (error) => {
              setIsGenerating(false);  // Stop generating on error
              console.error('Error streaming response:', error);
              setSession(prev => {
                const updatedSession = { ...prev };
                const updatedRounds = [...(updatedSession.rounds || [])];

                if (updatedRounds.length > 0) {
                  const lastRound = { ...updatedRounds[updatedRounds.length - 1] };
                  lastRound.assistantMessage = (lastRound.assistantMessage || '') +
                    "\n\nSorry, there was an error generating the rest of the response.";
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
        const updatedRounds = [...(updatedSession.rounds || [])];

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
      const updatedRounds = [...(updatedSession.rounds || [])];

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

      setSessions(prev => [...prev, newSession]);
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

      // Update local state if successful
      setSessions(prev => prev.map(s =>
        s.sessionId === updatedSession.sessionId ? updatedSession : s
      ));

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

      // Update local state if successful
      setSessions(prev => prev.filter(s => s.sessionId !== sessionToDelete.sessionId));

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
        setSession(response.data);

        // Also update the session in the sessions list
        setSessions(prev => prev.map(s =>
          s.sessionId === response.data?.sessionId ? response.data : s
        ) as API.SessionDto[]);
      }
    } catch (error) {
      console.error('Error refreshing session:', error);
    }
  };

  return (
    <ChatContext.Provider value={{
      session,
      sessions,
      loading,
      isGenerating,
      // Settings from storage
      // showMessageTimestamp: settings.showMessageTimestamp,
      // showModelName: settings.showModelName,
      // showTokenUsed: settings.showTokenUsed,
      // showTimeTaken: settings.showTimeTaken,
      // enableMarkdownRendering: settings.enableMarkdownRendering,
      // enableThinking: settings.enableThinking,
      // enableStreaming: settings.enableStreaming,
      // showOnlyActiveMessages: settings.showOnlyActiveMessages,
      // enableCollapsibleMessages: settings.enableCollapsibleMessages,
      // selectedModel: settings.selectedModel,
      // Setters
      setSession,
      setSessions,
      // setSidebarCollapsed,
      // setRightSidebarCollapsed,
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