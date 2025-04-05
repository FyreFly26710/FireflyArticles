import { useEffect, useState, useRef } from 'react'
import { Avatar, Typography, Grid, Button } from 'antd'
import { UserOutlined, RobotOutlined, SettingOutlined } from '@ant-design/icons'
import { format } from 'date-fns'
import Markdown from './Markdown'
import { cn } from '@/libs/utils'
import { Message as MessageType } from '@/types/chat'
import { useChat } from '@/app/aichat/context/ChatContext'

// Placeholder values for atoms
const showMessageTimestamp = true
const showModelName = true
const showTokenCount = true
const showWordCount = true
const showTokenUsed = true
const enableMarkdownRendering = true
const currentSessionPicUrl = null
const setOpenSettingWindow = (type: string) => console.log('Open settings:', type)

interface MessageProps {
    msg: MessageType
    collapseThreshold?: number
}

export default function Message(props: MessageProps) {
    const { msg, collapseThreshold} = props
    const needCollapse = collapseThreshold
        && (JSON.stringify(msg.content)).length > collapseThreshold
        && (JSON.stringify(msg.content)).length - collapseThreshold > 50
    const [isCollapsed, setIsCollapsed] = useState(needCollapse)

    const ref = useRef<HTMLDivElement>(null)

    const tips: string[] = []
    if (showTokenUsed && msg.role === 'assistant' && !msg.generating) {
        tips.push(`tokens used: ${msg.tokensUsed || 'unknown'}`)
    }
    if (showModelName && msg.role === 'assistant') {
        tips.push(`model: ${msg.model || 'unknown'}`)
    }



    useEffect(() => {
        if (msg.generating) {
            // Simple scroll to bottom implementation
            window.scrollTo(0, document.body.scrollHeight)
        }
    }, [msg.content])

    let content = msg.content
    if (typeof msg.content !== 'string') {
        content = JSON.stringify(msg.content)
    }
    if (msg.generating) {
        content += '...'
    }
    if (needCollapse && isCollapsed) {
        content = msg.content.slice(0, collapseThreshold) + '... '
    }

    const CollapseButton = (
        <span
            className="cursor-pointer inline-block font-bold text-blue-500 hover:text-white hover:bg-blue-500"
            onClick={() => setIsCollapsed(!isCollapsed)}
        >
            [{isCollapsed ? 'Expand' : 'Collapse'}]
        </span>
    )

    return (
        <div
            ref={ref}
            id={msg.id}
            key={msg.id}
            className={cn(
                'group/message',
                'px-4',
                msg.generating ? 'rendering' : 'render-done',
                {
                    user: 'bg-white',
                    system: 'bg-yellow-50',
                    assistant: 'bg-white',
                }[msg.role || 'user'],
            )}
        >
            <div className={cn(
                "flex items-start p-4",
                msg.role === 'user' ? "flex-row-reverse space-x-reverse space-x-4" : "flex-row space-x-4"
            )}>
                <div className={cn(
                    "mt-2",
                    msg.role === 'user' ? "ml-4" : "mr-4"
                )}>
                    {msg.role === 'assistant' ? (
                        currentSessionPicUrl ? (
                            <Avatar src={currentSessionPicUrl} size={28} />
                        ) : (
                            <Avatar icon={<RobotOutlined />} size={28} className="bg-primary" />
                        )
                    ) : msg.role === 'user' ? (
                        <Avatar
                            icon={<UserOutlined />}
                            size={28}
                            className="cursor-pointer"
                            onClick={() => setOpenSettingWindow('chat')}
                        />
                    ) : (
                        <Avatar
                            icon={<SettingOutlined />}
                            size={28}
                            className="bg-warning"
                        />
                    )}
                </div>
                <div className={cn(
                    "flex-1 min-w-0 pt-2",
                    msg.role === 'user' ? "text-right" : "text-left"
                )}>
                    {msg.role === 'user' ? (
                        <div className="flex justify-end">
                            <div className="bg-gray-100 rounded-lg p-3 max-w-[80%] break-words whitespace-pre-wrap text-left">
                                {content}
                            </div>
                        </div>
                    ) : (
                        <div className="mr-4 break-words whitespace-pre-wrap text-left">
                            {enableMarkdownRendering && !isCollapsed ? (
                                <Markdown>{content}</Markdown>
                            ) : (
                                <div>
                                    {content}
                                    {needCollapse && isCollapsed && CollapseButton}
                                </div>
                            )}
                            {needCollapse && !isCollapsed && CollapseButton}
                        </div>
                    )}
                    <Typography.Text type="secondary" className={cn(
                        "text-xs mt-2 block",
                        msg.role === 'user' ? "text-right" : "text-left"
                    )}>
                        {tips.join(', ')}
                    </Typography.Text>
                </div>
            </div>
        </div>
    )
}
