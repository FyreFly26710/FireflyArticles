'use client';

import { createContext, useContext, useState, ReactNode } from 'react';
import { Message, Session } from '@/types/chat';

interface ChatContextType {
  session: Session;
  sessions: Session[];
  sidebarCollapsed: boolean;
  setSession: (session: Session) => void;
  setSessions: (sessions: Session[]) => void;
  setSidebarCollapsed: (collapsed: boolean) => void;
  handleSendMessage: (message: Message) => Promise<void>;
  handleEditSession: (updatedSession: Session) => void;
  handleCreateSession: () => void;
  handleSelectSession: (selectedSession: Session) => void;
  handleEditSessionName: (updatedSession: Session) => void;
  handleDeleteSession: (sessionToDelete: Session) => void;
}

const ChatContext = createContext<ChatContextType | undefined>(undefined);

const mockApiCall = async (message: string): Promise<Message> => {
  await new Promise(resolve => setTimeout(resolve, 500));
  return {
    id: Date.now().toString(),
    content: message,
    role: 'assistant',
    timestamp: Date.now(),
    tokensUsed: 11,
    tokenCount: 22,
    wordCount: 33,
  };
};

export function ChatProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<Session>({
    id: '1',
    name: 'New Chat',
    messages: []
  });

  const [sessions, setSessions] = useState<Session[]>([{
    id: '1',
    name: 'New Chat',
    messages: []
  }]);

  const [sidebarCollapsed, setSidebarCollapsed] = useState(false);

  const handleSendMessage = async (message: Message) => {
    // Add user message to the session
    setSession(prev => ({
      ...prev,
      messages: [...prev.messages, message]
    }));

    try {
      // Create a placeholder message with generating state
      const placeholderResponse: Message = {
        id: Date.now().toString(),
        content: "",
        role: 'assistant',
        timestamp: Date.now(),
        generating: true
      };

      // Add the placeholder generating message
      setSession(prev => ({
        ...prev,
        messages: [...prev.messages, placeholderResponse]
      }));

      // Make the API call
      const assistantResponse = await mockApiCall(message.content);
      
      // Update the message with the response content and turn off generating state
      setSession(prev => {
        const updatedMessages = [...prev.messages];
        // Replace the placeholder with the actual response
        updatedMessages[updatedMessages.length - 1] = {
          ...assistantResponse,
          id: placeholderResponse.id, // Keep the same ID for animation continuity
          generating: false
        };
        return {
          ...prev,
          messages: updatedMessages
        };
      });
    } catch (error) {
      console.error('Error getting assistant response:', error);
      
      // Update the message to show an error
      setSession(prev => {
        const updatedMessages = [...prev.messages];
        if (updatedMessages[updatedMessages.length - 1]?.generating) {
          updatedMessages[updatedMessages.length - 1] = {
            ...updatedMessages[updatedMessages.length - 1],
            content: "Sorry, there was an error generating a response.",
            generating: false
          };
        }
        return {
          ...prev,
          messages: updatedMessages
        };
      });
    }
  };

  const handleEditSession = (updatedSession: Session) => {
    setSession(updatedSession);
  };

  const handleCreateSession = () => {
    const newSession: Session = {
      id: Date.now().toString(),
      name: 'New Chat',
      messages: []
    };
    setSessions(prev => [...prev, newSession]);
    setSession(newSession);
  };

  const handleSelectSession = (selectedSession: Session) => {
    setSession(selectedSession);
  };

  const handleEditSessionName = (updatedSession: Session) => {
    setSessions(prev => prev.map(s =>
      s.id === updatedSession.id ? updatedSession : s
    ));
    if (session.id === updatedSession.id) {
      setSession(updatedSession);
    }
  };

  const handleDeleteSession = (sessionToDelete: Session) => {
    setSessions(prev => prev.filter(s => s.id !== sessionToDelete.id));
    if (session.id === sessionToDelete.id) {
      setSession(sessions[0] || {
        id: Date.now().toString(),
        name: 'New Chat',
        type: 'chat',
        messages: []
      });
    }
  };

  return (
    <ChatContext.Provider value={{
      session,
      sessions,
      sidebarCollapsed,
      setSession,
      setSessions,
      setSidebarCollapsed,
      handleSendMessage,
      handleEditSession,
      handleCreateSession,
      handleSelectSession,
      handleEditSessionName,
      handleDeleteSession,
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