import { useState } from 'react'
import { Input } from 'antd'
import { Session } from '@/types/chat'
import SessionItem from './SessionItem'
import { cn } from '@/libs/utils'

interface SessionListProps {
    sessions: Session[]
    currentSessionId: string
    onSelectSession: (session: Session) => void
    onEditSession: (session: Session) => void
    onDeleteSession: (session: Session) => void
    onCreateSession: () => void
    className?: string
}

export default function SessionList({
    sessions,
    currentSessionId,
    onSelectSession,
    onEditSession,
    onDeleteSession,
    onCreateSession,
    className
}: SessionListProps) {
    const [searchText, setSearchText] = useState('')

    const filteredSessions = sessions.filter(session =>
        session.name.toLowerCase().includes(searchText.toLowerCase())
    )

    return (
        <div className={cn("flex flex-col h-full p-4 bg-white", className)}>

            <div className="flex-1 overflow-y-auto pr-2 [&::-webkit-scrollbar]:w-1.5 [&::-webkit-scrollbar-thumb]:bg-gray-300 [&::-webkit-scrollbar-thumb]:rounded [&::-webkit-scrollbar-track]:bg-transparent">
                {filteredSessions.map(session => (
                    <SessionItem
                        key={session.id}
                        session={session}
                        isActive={session.id === currentSessionId}
                        onSelect={onSelectSession}
                        onEdit={onEditSession}
                        onDelete={onDeleteSession}
                    />
                ))}
            </div>
        </div>
    )
}
