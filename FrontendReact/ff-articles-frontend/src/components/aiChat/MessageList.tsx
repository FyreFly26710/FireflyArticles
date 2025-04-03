import { useEffect, useRef } from 'react'
import Message from './Message'
import { cn } from '@/libs/utils'
import { Message as MessageType, Session } from '@/types/chat'

interface MessageListProps {
    messages: MessageType[]
    session: Session
    className?: string
}

export default function MessageList({ messages, session, className }: MessageListProps) {
    const ref = useRef<HTMLDivElement>(null)

    // Simple scroll to bottom on new messages
    useEffect(() => {
        if (ref.current) {
            ref.current.scrollTop = ref.current.scrollHeight
        }
    }, [messages])

    return (
        <div
            className={cn(
                'w-full h-full bg-white px-4',
                className
            )}
            ref={ref}
        >
            {messages.map((msg, index) => (
                <Message
                    key={msg.id}
                    id={msg.id}
                    msg={msg}
                    sessionId={session.id}
                    sessionType={session.type}
                    className={index === 0 ? 'pt-4' : ''}
                    collapseThreshold={msg.role === 'system' ? 150 : undefined}
                />
            ))}
        </div>
    )
}
