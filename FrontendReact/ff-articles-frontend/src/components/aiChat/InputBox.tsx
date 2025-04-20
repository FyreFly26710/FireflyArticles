'use client'
import React, { useRef, useState } from 'react'
import { Button, Input } from 'antd'
import { SendOutlined, SettingOutlined } from '@ant-design/icons'
import { useChat } from '@/app/aichat/context/ChatContext'
import InputSettings from './InputSettings'

export default function InputBox(){
    const [messageInput, setMessageInput] = useState('')
    const [showSettings, setShowSettings] = useState(false)
    const inputRef = useRef<HTMLTextAreaElement>(null)
    
    const { 
      handleSendMessage, 
      session,
    } = useChat()

    const handleSubmit = () => {
        if (messageInput.trim() === '') {
            return
        }
        const newMessage: API.ChatRoundCreateRequest = {
            sessionId: session.sessionId,
            SessionTimeStamp: session.timestamp,
            userMessage: messageInput,
            model: 'deepseek'
        }
        handleSendMessage(newMessage)
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
    }

    return (
        <div className="h-full flex flex-col p-4 gap-3">
            {/* Settings Panel */}
            <InputSettings visible={showSettings} />

            {/* Input Area */}
            <div className="flex-shrink-0 rounded-lg border border-gray-200 p-3 bg-white shadow-sm w-full">
                <div className="flex">
                    <div className="flex-grow pr-2">
                        <Input.TextArea
                            className="w-full resize-none border-none outline-none bg-transparent h-full"
                            value={messageInput}
                            onChange={(e) => setMessageInput(e.target.value)}
                            onKeyDown={onKeyDown}
                            ref={inputRef}
                            placeholder="Type your question here..."
                            autoSize={{ minRows: 3, maxRows: 5 }}
                        />
                    </div>
                    
                    <div className="flex flex-shrink-0 flex-col gap-2">
                        <Button
                            type="text"
                            icon={<SettingOutlined />}
                            onClick={() => setShowSettings(!showSettings)}
                            title="Settings"
                        />
                        <Button
                            type="primary"
                            icon={<SendOutlined />}
                            onClick={handleSubmit}
                            style={{ minHeight: '38px' }}
                        />
                    </div>
                </div>
            </div>
        </div>
    )
}
