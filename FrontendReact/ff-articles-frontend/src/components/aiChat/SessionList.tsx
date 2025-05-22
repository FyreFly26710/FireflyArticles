import { useEffect, useState } from 'react'
import { Input } from 'antd'
import SessionItem from './SessionItem'
import { useChat } from '@/hooks/useChat'

export default function SessionList() {
    // const [searchText, setSearchText] = useState('')

    const {
        session,
        sessions,
        handleSelectSession,
        handleEditSessionName,
        handleDeleteSession,
        handleCreateSession
    } = useChat();

    return (
        <div className={"flex flex-col h-full p-4 bg-white"}>

            <div className="flex-1 overflow-y-auto pr-2 [&::-webkit-scrollbar]:w-1.5 [&::-webkit-scrollbar-thumb]:bg-gray-300 [&::-webkit-scrollbar-thumb]:rounded [&::-webkit-scrollbar-track]:bg-transparent">
                {sessions.map(sessionItem => (
                    <SessionItem
                        key={sessionItem.sessionId}
                        session={sessionItem}
                        isActive={sessionItem.sessionId === session.sessionId}
                        onSelect={handleSelectSession}
                        onEdit={handleEditSessionName}
                        onDelete={handleDeleteSession}
                    />
                ))}
            </div>
        </div>
    )
}
