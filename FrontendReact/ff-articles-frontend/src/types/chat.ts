export interface Message {
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

export interface Session {
    id: string
    name: string
    messages: Message[]
}

// export interface InputBoxProps {
//     currentSessionId: string
//     // currentSessionType: 'chat' | 'system'
//     onSendMessage: (message: Message) => void
//     onOpenSettings?: () => void
// } 