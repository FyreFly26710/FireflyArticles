import { useEffect, useRef } from 'react'
import Message from './Message'
import { useChat } from '@/app/aichat/context/ChatContext'


export default function MessageList() {
    const ref = useRef<HTMLDivElement>(null)
    const { session } = useChat()
    const messages = session.messages

    // Simple scroll to bottom on new messages
    useEffect(() => {
        if (ref.current) {
            ref.current.scrollTop = ref.current.scrollHeight
        }
    }, [messages])

    return (
        <div
            className="w-full h-full bg-white px-4"
            ref={ref}
        >
            {messages.map((msg, index) => (
                <Message
                    key={msg.id}
                    msg={msg}
                    collapseThreshold={msg.role === 'system' ? 150 : undefined}
                />
            ))}
        </div>
    )
}
