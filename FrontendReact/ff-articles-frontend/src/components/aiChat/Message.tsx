import { useEffect, useState, useRef } from 'react'
import { Avatar, Typography, Grid, Button } from 'antd'
import { UserOutlined, RobotOutlined, SettingOutlined } from '@ant-design/icons'
import { format } from 'date-fns'
import Markdown from './Markdown'
import { cn } from '@/libs/utils'

// Placeholder values for atoms
const showMessageTimestamp = true
const showModelName = true
const showTokenCount = true
const showWordCount = true
const showTokenUsed = true
const enableMarkdownRendering = true
const currentSessionPicUrl = null
const setOpenSettingWindow = (type: string) => console.log('Open settings:', type)

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

interface Props {
    id?: string
    sessionId: string
    sessionType: 'chat' | 'system'
    msg: Message
    className?: string
    collapseThreshold?: number
    hiddenButtonGroup?: boolean
    small?: boolean
}

export default function Message(props: Props) {
    const { msg, className, collapseThreshold, hiddenButtonGroup, small } = props

    const needCollapse = collapseThreshold
        && (JSON.stringify(msg.content)).length > collapseThreshold
        && (JSON.stringify(msg.content)).length - collapseThreshold > 50
    const [isCollapsed, setIsCollapsed] = useState(needCollapse)

    const ref = useRef<HTMLDivElement>(null)

    const tips: string[] = []
    if (props.sessionType === 'chat' || !props.sessionType) {
        if (showWordCount && !msg.generating) {
            tips.push(`word count: ${msg.wordCount || msg.content.split(' ').length}`)
        }
        if (showTokenCount && !msg.generating) {
            tips.push(`token count: ${msg.tokenCount || Math.ceil(msg.content.length / 4)}`)
        }
        if (showTokenUsed && msg.role === 'assistant' && !msg.generating) {
            tips.push(`tokens used: ${msg.tokensUsed || 'unknown'}`)
        }
        if (showModelName && props.msg.role === 'assistant') {
            tips.push(`model: ${props.msg.model || 'unknown'}`)
        }
    }

    if (showMessageTimestamp && msg.timestamp !== undefined) {
        const date = new Date(msg.timestamp)
        let messageTimestamp: string
        if (date.toDateString() === new Date().toDateString()) {
            messageTimestamp = format(date, 'HH:mm')
        } else if (date.getFullYear() === new Date().getFullYear()) {
            messageTimestamp = format(date, 'MM-dd HH:mm')
        } else {
            messageTimestamp = format(date, 'yyyy-MM-dd HH:mm')
        }
        tips.push('time: ' + messageTimestamp)
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
            id={props.id}
            key={msg.id}
            className={cn(
                'group/message',
                'px-2',
                msg.generating ? 'rendering' : 'render-done',
                {
                    user: 'bg-gray-100',
                    system: 'bg-yellow-50',
                    assistant: 'bg-white',
                }[msg.role || 'user'],
                className,
            )}
        >
            <div className="flex items-start space-x-4 p-4">
                <div className="mt-2">
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
                <div className="flex-1 min-w-0">
                    <div className={cn('msg-content', { 'text-sm': small })}>
                        {enableMarkdownRendering && !isCollapsed ? (
                            <Markdown>{content}</Markdown>
                        ) : (
                            <div>
                                {content}
                                {needCollapse && isCollapsed && CollapseButton}
                            </div>
                        )}
                    </div>
                    {needCollapse && !isCollapsed && CollapseButton}
                    <Typography.Text type="secondary" className="text-xs">
                        {tips.join(', ')}
                    </Typography.Text>
                </div>
            </div>
        </div>
    )
}
