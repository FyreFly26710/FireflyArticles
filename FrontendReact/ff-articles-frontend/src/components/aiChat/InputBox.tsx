'use client'
import React, { useRef, useState } from 'react'
import { Button, Input, Tooltip } from 'antd'
import { SendOutlined, SettingOutlined } from '@ant-design/icons'
import { cn } from '@/libs/utils'

interface Message {
    id: string
    content: string
    role: 'user' | 'assistant' | 'system'
    timestamp?: number
    generating?: boolean
}

interface Props {
    currentSessionId: string
    currentSessionType: 'chat' | 'system'
    onSendMessage: (message: Message) => void
    onOpenSettings?: () => void
}

export default function InputBox({
    currentSessionId,
    currentSessionType,
    onSendMessage,
    onOpenSettings
}: Props) {
    const [messageInput, setMessageInput] = useState('')
    const inputRef = useRef<HTMLTextAreaElement>(null)
    const [easterEgg, setEasterEgg] = useState(false)

    const handleSubmit = (needGenerating = true) => {
        if (messageInput.trim() === '') {
            return
        }
        const newMessage: Message = {
            id: Date.now().toString(),
            content: messageInput,
            role: 'user',
            timestamp: Date.now()
        }
        onSendMessage(newMessage)
        setMessageInput('')
    }

    const onKeyDown = (event: React.KeyboardEvent<HTMLTextAreaElement>) => {
        if (
            event.key === 'Enter' &&
            !event.shiftKey &&
            !event.ctrlKey &&
            !event.altKey &&
            !event.metaKey
        ) {
            event.preventDefault()
            handleSubmit()
            return
        }
        if (event.key === 'Enter' && event.ctrlKey) {
            event.preventDefault()
            handleSubmit(false)
            return
        }
    }

    return (
        <div className="border-t border-gray-200 pl-2 pr-4">
            <div className="w-full mx-auto flex flex-col">
                <div className="flex flex-row flex-nowrap justify-between py-1">
                    <div className="flex flex-row items-center">
                        <Tooltip title="Click for fun!">
                            <Button
                                type="text"
                                className="mr-2 hover:bg-transparent"
                                onClick={() => {
                                    setEasterEgg(true)
                                    setTimeout(() => setEasterEgg(false), 1000)
                                }}
                            >
                                <img
                                    className={cn('w-5 h-5', easterEgg ? 'animate-spin' : '')}
                                    src="/icon.png"
                                    alt="icon"
                                />
                            </Button>
                        </Tooltip>
                        {onOpenSettings && (
                            <Tooltip title="Customize settings for the current conversation">
                                <Button
                                    type="text"
                                    className="mr-2"
                                    icon={<SettingOutlined />}
                                    onClick={onOpenSettings}
                                />
                            </Tooltip>
                        )}
                    </div>
                    <div className="flex flex-row items-center">
                        <Tooltip title="[Enter] send, [Shift+Enter] line break, [Ctrl+Enter] send without generating">
                            <Button
                                type="primary"
                                className="w-8 ml-2"
                                icon={<SendOutlined />}
                                onClick={() => handleSubmit()}
                            />
                        </Tooltip>
                    </div>
                </div>
                <div className="w-full pl-1 pb-2">
                    <Input.TextArea
                        className="w-full resize-none border-none outline-none bg-transparent p-1"
                        value={messageInput}
                        onChange={(e) => setMessageInput(e.target.value)}
                        onKeyDown={onKeyDown}
                        ref={inputRef}
                        placeholder="Type your question here..."
                        autoSize={{ minRows: 3, maxRows: 4 }}
                    />
                </div>
            </div>
        </div>
    )
}
