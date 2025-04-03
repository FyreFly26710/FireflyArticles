'use client'
import React, { useRef, useState } from 'react'
import { Button, Input, Tooltip } from 'antd'
import { SendOutlined, SettingOutlined } from '@ant-design/icons'
import { Message, InputBoxProps } from '@/types/chat'

export default function InputBox({
    currentSessionId,
    currentSessionType,
    onSendMessage,
    onOpenSettings
}: InputBoxProps) {
    const [messageInput, setMessageInput] = useState('')
    const inputRef = useRef<HTMLTextAreaElement>(null)

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
        <div className="p-2">
            <div className="flex flex-col gap-2">
                <div className="flex justify-between items-center bg-white">
                    {onOpenSettings && (
                        <Tooltip title="Customize settings for the current conversation">
                            <Button
                                type="text"
                                icon={<SettingOutlined />}
                                onClick={onOpenSettings}
                            />
                        </Tooltip>
                    )}
                    <Tooltip title="[Enter] send, [Shift+Enter] line break, [Ctrl+Enter] send without generating">
                        <Button
                            type="primary"
                            className="w-8"
                            icon={<SendOutlined />}
                            onClick={() => handleSubmit()}
                        />
                    </Tooltip>
                </div>
                <Input.TextArea
                    className="w-full resize-none border-none outline-none bg-transparent"
                    value={messageInput}
                    onChange={(e) => setMessageInput(e.target.value)}
                    onKeyDown={onKeyDown}
                    ref={inputRef}
                    placeholder="Type your question here..."
                    autoSize={{ minRows: 3, maxRows: 4 }}
                />
            </div>
        </div>
    )
}
