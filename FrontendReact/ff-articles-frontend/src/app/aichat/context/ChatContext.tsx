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
import { storage, ChatSettings } from '@/stores/storage';

interface ChatContextType {
  session: API.SessionDto;
  sessions: API.SessionDto[];
  sidebarCollapsed: boolean;
  rightSidebarCollapsed: boolean;
  loading: boolean;
  // Settings
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
  // Setters
  setSession: (session: API.SessionDto) => void;
  setSessions: (sessions: API.SessionDto[]) => void;
  setSidebarCollapsed: (collapsed: boolean) => void;
  setRightSidebarCollapsed: (collapsed: boolean) => void;
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
  createdAt: new Date().toISOString()
});

// Default settings
const defaultSettings: ChatSettings = {
    showMessageTimestamp: true,
    showModelName: true,
    showTokenUsed: true,
    showTimeTaken: true,
    enableMarkdownRendering: true,
    enableThinking: true,
    enableStreaming: true,
    showOnlyActiveMessages: false,
    enableCollapsibleMessages: true,
    selectedModel: 'deepseek'
};

export function ChatProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<API.SessionDto>(createEmptySession());
  const [sessions, setSessions] = useState<API.SessionDto[]>([]);
  const [sidebarCollapsed, setSidebarCollapsed] = useState(false);
  const [rightSidebarCollapsed, setRightSidebarCollapsed] = useState(false);
  const [loading, setLoading] = useState(true);

  // Get settings from storage
  const settings = storage.getChatSettings() || defaultSettings;

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
        const defaultSession = createEmptySession();
        defaultSession.sessionId = Date.now();
        setSessions([defaultSession]);
        setSession(defaultSession);
      } finally {
        setLoading(false);
      }
    };

    fetchSessions();
  }, []);

  const handleSendMessage = async (messageRequest: API.ChatRoundCreateRequest) => {
    try {
      // Create a placeholder response for UI feedback
      const placeholderChatRound: API.ChatRoundDto = {
        sessionId: messageRequest.sessionId,
        chatRoundId: Date.now(),
        userMessage: messageRequest.userMessage,
        assistantMessage: "",
        model: messageRequest.model || "",
        createdAt: new Date().toISOString(),
        promptTokens: 0,
        completionTokens: 0,
        totalTokens: 0,
        timeTaken: 0,
        isActive: true
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

      // Update the request with streaming setting
      messageRequest.enableStreaming = settings.enableStreaming;

      if (settings.enableStreaming) {
        // Track the abort controller to allow canceling the stream
        let abortStream: (() => void) | null = null;
        
        // Handle streaming response
        abortStream = apiChatRoundStreamResponse(
          messageRequest,
          {
            onInit: (data) => {
              // Update placeholder with the initial data
              setSession(prev => {
                const updatedSession = { ...prev };
                const updatedRounds = [...(updatedSession.rounds || [])];
                
                if (updatedRounds.length > 0) {
                  const lastRound = { ...updatedRounds[updatedRounds.length - 1] };
                  lastRound.chatRoundId = data.chatRoundId;
                  lastRound.sessionId = data.sessionId;
                  updatedRounds[updatedRounds.length - 1] = lastRound;
                }
                
                updatedSession.rounds = updatedRounds;
                return updatedSession;
              });
            },
            onChunk: (content) => {
              // Progressively update the assistant's response
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
            onTokens: (promptTokens, completionTokens) => {
              // Update token usage information as it becomes available
              setSession(prev => {
                const updatedSession = { ...prev };
                const updatedRounds = [...(updatedSession.rounds || [])];
                
                if (updatedRounds.length > 0) {
                  const lastRound = { ...updatedRounds[updatedRounds.length - 1] };
                  lastRound.promptTokens = promptTokens;
                  lastRound.completionTokens = completionTokens;
                  lastRound.totalTokens = promptTokens + completionTokens;
                  updatedRounds[updatedRounds.length - 1] = lastRound;
                }
                
                updatedSession.rounds = updatedRounds;
                return updatedSession;
              });
            },
            onDone: (data) => {
              // Replace the placeholder with the final response
              setSession(prev => {
                const updatedSession = { ...prev };
                const updatedRounds = [...(updatedSession.rounds || [])];
                
                if (updatedRounds.length > 0) {
                  updatedRounds[updatedRounds.length - 1] = data;
                }
                
                // Update session ID if API created a new session
                const sessionIdChanged = updatedSession.sessionId !== data.sessionId;
                if (sessionIdChanged) {
                  updatedSession.sessionId = data.sessionId;
                  
                  // If session ID changed, update the sessions array too
                  setSessions(prevSessions => {
                    // Remove the old session with the previous ID
                    const filteredSessions = prevSessions.filter(s => s.sessionId !== prev.sessionId);
                    
                    // Check if the new session already exists
                    const sessionIndex = filteredSessions.findIndex(s => s.sessionId === data.sessionId);
                    
                    // If the session exists in the array, update it
                    if (sessionIndex >= 0) {
                      const updatedSessions = [...filteredSessions];
                      updatedSessions[sessionIndex] = updatedSession;
                      return updatedSessions;
                    } 
                    // If it's a completely new session, add it to the array
                    else {
                      return [...filteredSessions, updatedSession];
                    }
                  });
                }
                
                updatedSession.rounds = updatedRounds;
                updatedSession.roundCount = updatedRounds.length;
                return updatedSession;
              });
            },
            onError: (error) => {
              console.error('Error streaming response:', error);
              
              // Update UI to show error
              setSession(prev => {
                const updatedSession = { ...prev };
                const updatedRounds = [...(updatedSession.rounds || [])];
                
                if (updatedRounds.length > 0) {
                  const lastRound = updatedRounds[updatedRounds.length - 1];
                  updatedRounds[updatedRounds.length - 1] = {
                    ...lastRound,
                    assistantMessage: lastRound.assistantMessage + "\n\nSorry, there was an error generating the rest of the response."
                  };
                }
                
                updatedSession.rounds = updatedRounds;
                return updatedSession;
              });
            }
          }
        );
      } else {
        // Make the regular API call for non-streaming
        const response = await apiChatRoundAddByRequest(messageRequest);
        
        if (!response.data) {
          throw new Error('No data returned from API');
        }

        // Update with the actual response
        setSession(prev => {
          const updatedSession = { ...prev };
          const updatedRounds = [...(updatedSession.rounds || [])];
          
          // Replace the placeholder with the actual response
          if (updatedRounds.length > 0 && response.data) {
            updatedRounds[updatedRounds.length - 1] = response.data;
          } else if (response.data) {
            updatedRounds.push(response.data);
          }
          
          // Update session ID if API created a new session
          const newSessionId = response.data?.sessionId ?? messageRequest.sessionId;
          const sessionIdChanged = updatedSession.sessionId !== newSessionId;
          updatedSession.sessionId = newSessionId;
          updatedSession.rounds = updatedRounds;
          updatedSession.roundCount = updatedRounds.length;
          
          // If session ID changed, update the sessions array too
          if (sessionIdChanged) {
            setSessions(prevSessions => {
              // First, remove the old session with the previous ID
              const filteredSessions = prevSessions.filter(s => s.sessionId !== prev.sessionId);
              
              // Then check if the new session already exists
              const sessionIndex = filteredSessions.findIndex(s => s.sessionId === newSessionId);
              
              // If the session exists in the array, update it
              if (sessionIndex >= 0) {
                const updatedSessions = [...filteredSessions];
                updatedSessions[sessionIndex] = updatedSession;
                return updatedSessions;
              } 
              // If it's a completely new session, add it to the array
              else {
                return [...filteredSessions, updatedSession];
              }
            });
          }
          
          return updatedSession;
        });
      }
    } catch (error) {
      console.error('Error getting assistant response:', error);
      
      // Update UI to show error
      setSession(prev => {
        const updatedSession = { ...prev };
        const updatedRounds = [...(updatedSession.rounds || [])];
        
        if (updatedRounds.length > 0) {
          const lastRound = updatedRounds[updatedRounds.length - 1];
          updatedRounds[updatedRounds.length - 1] = {
            ...lastRound,
            assistantMessage: "Sorry, there was an error generating a response."
          };
        }
        
        updatedSession.rounds = updatedRounds;
        return updatedSession;
      });
    }
  };

  const handleCreateSession = async () => {
    try {
      // Default session name
      const name = 'New Chat';
      
      // Create a new session
      const newSession: API.SessionDto = {
        sessionId: Date.now(),
        sessionName: name,
        rounds: [],
        roundCount: 0,
        createdAt: new Date().toISOString()
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
      const response = await apiSessionGetById({ id: selectedSession.sessionId });
      if (response.data) {
        setSession(response.data);
      } else {
        setSession(selectedSession);
      }
    } catch (error) {
      console.error('Error selecting session:', error);
      setSession(selectedSession);
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
      if (session.sessionId === updatedSession.sessionId) {
        setSession(updatedSession);
      }
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
      console.log('session', session);
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
      sidebarCollapsed,
      rightSidebarCollapsed,
      loading,
      // Settings from storage
      showMessageTimestamp: settings.showMessageTimestamp,
      showModelName: settings.showModelName,
      showTokenUsed: settings.showTokenUsed,
      showTimeTaken: settings.showTimeTaken,
      enableMarkdownRendering: settings.enableMarkdownRendering,
      enableThinking: settings.enableThinking,
      enableStreaming: settings.enableStreaming,
      showOnlyActiveMessages: settings.showOnlyActiveMessages,
      enableCollapsibleMessages: settings.enableCollapsibleMessages,
      selectedModel: settings.selectedModel,
      // Setters
      setSession,
      setSessions,
      setSidebarCollapsed,
      setRightSidebarCollapsed,
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