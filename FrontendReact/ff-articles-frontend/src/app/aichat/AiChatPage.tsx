'use client';

import { useState } from 'react'
import Header from '@/components/aiChat/Header'
import MessageList from '@/components/aiChat/MessageList'
import InputBox from '@/components/aiChat/InputBox'

interface Message {
    id: string
    content: string
    role: 'user' | 'assistant' | 'system'
    timestamp?: number
    generating?: boolean
    model?: string
    tokenCount?: number
    tokensUsed?: number
    wordCount?: number
}

interface Session {
    id: string
    name: string
    type: 'chat' | 'system'
    messages: Message[]
}

export default function AiChatPage() {
    const [session, setSession] = useState<Session>({
        id: '1',
        name: 'New Chat',
        type: 'chat',
        messages: []
    })

    const handleSendMessage = (message: Message) => {
        setSession(prev => ({
            ...prev,
            messages: [...prev.messages, message]
        }))
    }

    const handleEditSession = (updatedSession: Session) => {
        setSession(updatedSession)
    }

    return (
        <div className="flex flex-col h-full w-full">
            <Header
                session={session}
                onEditSession={handleEditSession}
            />
            <MessageList
                messages={session.messages}
                session={session}
            />
            <InputBox
                currentSessionId={session.id}
                currentSessionType={session.type}
                onSendMessage={handleSendMessage}
                onOpenSettings={() => {
                    // Implement settings dialog
                    console.log('Open settings')
                }}
            />
        </div>
    )
}
