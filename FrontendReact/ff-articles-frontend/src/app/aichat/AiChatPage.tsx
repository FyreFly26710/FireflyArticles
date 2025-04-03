'use client';

import { useState } from 'react'
import Header from '@/components/aiChat/Header'
import MessageList from '@/components/aiChat/MessageList'
import InputBox from '@/components/aiChat/InputBox'
import Sidebar from '@/components/aiChat/Sidebar'
import { Message, Session } from '@/types/chat'

const mockApiCall = async (message: string): Promise<Message> => {
    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 500));

    // Return mock assistant response
    return {
        id: Date.now().toString(),
        content: message, // For now, just echo back the same message
        role: 'assistant',
        timestamp: Date.now(),
        tokensUsed: 11,
        tokenCount: 22,
        wordCount: 33,
    };
};

export default function AiChatPage() {
    const [session, setSession] = useState<Session>({
        id: '1',
        name: 'New Chat',
        type: 'chat',
        messages: []
    })

    const [sessions, setSessions] = useState<Session[]>([{
        id: '1',
        name: 'New Chat',
        type: 'chat',
        messages: []
    }])

    const [sidebarCollapsed, setSidebarCollapsed] = useState(false)

    const handleSendMessage = async (message: Message) => {
        // Add user message immediately
        setSession(prev => ({
            ...prev,
            messages: [...prev.messages, message]
        }));

        try {
            // Get assistant response
            const assistantResponse = await mockApiCall(message.content);

            // Add assistant message
            setSession(prev => ({
                ...prev,
                messages: [...prev.messages, assistantResponse]
            }));
        } catch (error) {
            console.error('Error getting assistant response:', error);
        }
    }

    const handleEditSession = (updatedSession: Session) => {
        setSession(updatedSession)
    }

    const handleCreateSession = () => {
        const newSession: Session = {
            id: Date.now().toString(),
            name: 'New Chat',
            type: 'chat',
            messages: []
        }
        setSessions(prev => [...prev, newSession])
        setSession(newSession)
    }

    const handleSelectSession = (selectedSession: Session) => {
        setSession(selectedSession)
    }

    const handleEditSessionName = (updatedSession: Session) => {
        setSessions(prev => prev.map(s =>
            s.id === updatedSession.id ? updatedSession : s
        ))
        if (session.id === updatedSession.id) {
            setSession(updatedSession)
        }
    }

    const handleDeleteSession = (sessionToDelete: Session) => {
        setSessions(prev => prev.filter(s => s.id !== sessionToDelete.id))
        if (session.id === sessionToDelete.id) {
            setSession(sessions[0] || {
                id: Date.now().toString(),
                name: 'New Chat',
                type: 'chat',
                messages: []
            })
        }
    }

    return (
        <div className="h-screen w-screen bg-white">
            <Sidebar
                openCopilotWindow={() => console.log('Open copilot window')}
                openAboutWindow={() => console.log('Open about window')}
                setOpenSettingWindow={() => console.log('Open settings window')}
                sessions={sessions}
                currentSessionId={session.id}
                onSelectSession={handleSelectSession}
                onEditSession={handleEditSessionName}
                onDeleteSession={handleDeleteSession}
                onCreateSession={handleCreateSession}
                onCollapse={setSidebarCollapsed}
            />

            <div className={`transition-all duration-300 ${sidebarCollapsed ? 'ml-[80px]' : 'ml-[240px]'} h-screen bg-white`}>
                {/* <Header session={session} /> */}

                {/* Scrollable message area */}
                <div className="h-[calc(100vh-80px)] overflow-y-auto bg-white">
                    <div className="max-w-4xl mx-auto px-4">
                        <MessageList messages={session.messages} session={session} />
                    </div>
                </div>

                {/* Fixed input box */}
                <div className={`fixed bottom-0 ${sidebarCollapsed ? 'left-[80px]' : 'left-[240px]'} right-0 bg-white`}>
                    <div className="max-w-4xl mx-auto px-4">
                        <InputBox
                            currentSessionId={session.id}
                            currentSessionType={session.type}
                            onSendMessage={handleSendMessage}
                            onOpenSettings={() => console.log("Open settings")}
                        />
                    </div>
                </div>
            </div>
        </div>
    )
}
