import { useEffect, useRef } from 'react'
import Message from './Message'
import { cn } from '@/libs/utils'

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
    type: 'chat' | 'system'
}

interface Props {
    messages: Message[]
    session: Session
    className?: string
}

export default function MessageList({ messages, session, className }: Props) {
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
                'overflow-y-auto w-full h-full pr-0 pl-0',
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
