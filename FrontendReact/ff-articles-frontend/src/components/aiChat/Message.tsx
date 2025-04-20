import { useEffect, useState, useRef } from 'react'
import { Avatar, Typography, Divider } from 'antd'
import { UserOutlined, RobotOutlined } from '@ant-design/icons'
import Markdown from './Markdown'
import { storage } from '@/stores/storage'
import { useChat } from '@/app/aichat/context/ChatContext'

const setOpenSettingWindow = (type: string) => console.log('Open settings:', type)

interface MessageProps {
    chatRound: API.ChatRoundDto
    collapseThreshold?: number
    displayMode?: 'user' | 'assistant' | 'both'
}

export default function Message(props: MessageProps) {
    const { chatRound, collapseThreshold, displayMode = 'both' } = props
    
    // Get settings from context
    const displaySettings = storage.getChatDisplaySettings();
    const { isGenerating, session } = useChat();
    
    // Determine if this is the latest message
    const isLatestMessage = session.rounds && 
        session.rounds.length > 0 && 
        session.rounds[session.rounds.length - 1].chatRoundId === chatRound.chatRoundId;
    
    // Force rerender when generation completes for the latest message
    const [, forceUpdate] = useState({});
    useEffect(() => {
        if (isLatestMessage && !isGenerating) {
            console.log('chatRound', chatRound);
            forceUpdate({});
        }
    }, [isGenerating, isLatestMessage]);
    
    // Determine if the message is inactive
    const isInactive = !chatRound.isActive;
    
    // Get user and assistant content
    const userContent = chatRound.userMessage || "";
    const assistantContent = chatRound.assistantMessage || "";
    
    // Only apply collapse if the feature is enabled
    const shouldUseCollapse = displaySettings.enableCollapsibleMessages && collapseThreshold !== undefined;
    const assistantNeedsCollapse = shouldUseCollapse && 
        assistantContent.length > collapseThreshold && 
        assistantContent.length - collapseThreshold > 50;
    
    // Collapse state for user and assistant messages
    const [isAssistantCollapsed, setIsAssistantCollapsed] = useState(assistantNeedsCollapse);

    const ref = useRef<HTMLDivElement>(null)
    
    // Update collapse state when collapsible setting changes
    useEffect(() => {
        setIsAssistantCollapsed(assistantNeedsCollapse);
    }, [displaySettings.enableCollapsibleMessages, assistantNeedsCollapse]);

    // Prepare metadata tips for assistant message
    const tips: string[] = []
    if (displaySettings.showTokenUsed && !isGenerating) {
        tips.push(`tokens: input ${chatRound.promptTokens} output ${chatRound.completionTokens}`);
    }
    if (displaySettings.showModelName) {
        tips.push(`model: ${chatRound.model || 'unknown'}`);
    }
    if (displaySettings.showTimeTaken && !isGenerating) {
        tips.push(`time: ${Math.round(chatRound.timeTaken/1000)}s`);
    }
    if (displaySettings.showMessageTimestamp) {
        tips.push(`${new Date(chatRound.updateTime).toLocaleString()}`);
    }
    if (isInactive) {
        tips.push(`status: inactive`);
    }

    // Auto-scroll to bottom when message is being generated
    // useEffect(() => {
    //     if (isGenerating) {
    //         window.scrollTo(0, document.body.scrollHeight)
    //     }
    // }, [isGenerating, assistantContent])

    // Format content for display
    let formattedUserContent = userContent;
    
    let formattedAssistantContent = assistantContent;
    if (isGenerating) {
        formattedAssistantContent += '...';
    }
    if (assistantNeedsCollapse && isAssistantCollapsed) {
        formattedAssistantContent = assistantContent.slice(0, collapseThreshold) + '... '
    }

    const AssistantCollapseButton = assistantNeedsCollapse && (
        <span
            className="cursor-pointer inline-block font-bold text-blue-500 hover:text-white hover:bg-blue-500"
            onClick={() => setIsAssistantCollapsed(!isAssistantCollapsed)}
        >
            [{isAssistantCollapsed ? 'Expand' : 'Collapse'}]
        </span>
    )

    return (
        <div ref={ref} className={`group/message ${isInactive ? 'opacity-75' : ''} relative`}>
            {/* Inactive label */}
            {isInactive && (
                <div className="absolute top-0 left-0 bg-gray-200 text-gray-600 text-xs px-2 py-0.5 rounded-br-md z-10">
                    Inactive
                </div>
            )}
            
            {/* User Message - Only show if in 'user' or 'both' display mode */}
            {(displayMode === 'user' || displayMode === 'both') && (
                <div className="flex items-start py-2 flex-row-reverse">
                    <Avatar
                        icon={<UserOutlined />}
                        size={28}
                        className="cursor-pointer mt-1"
                        onClick={() => setOpenSettingWindow('chat')}
                    />
                    <div className="flex-1 text-right mr-4">
                        <div className={`inline-block bg-gray-100 rounded-lg p-3 max-w-[80%] break-words whitespace-pre-wrap text-left ${isInactive ? 'border border-gray-300' : ''}`}>
                            {formattedUserContent}
                        </div>
                    </div>
                </div>
            )}

            {/* Assistant Message - Only show if in 'assistant' or 'both' display mode */}
            {(displayMode === 'assistant' || displayMode === 'both') && (
                <div className={`${isGenerating ? 'rendering' : 'render-done'} py-2`}>
                    <div className="flex items-start">
                        <Avatar icon={<RobotOutlined />} size={28} className="bg-primary mt-1" />
                        <div className="flex-1 ml-4">
                            <div className={`mr-4 break-words whitespace-pre-wrap text-left ${isInactive ? 'text-gray-500' : ''}`}>
                                {displaySettings.enableMarkdownRendering && !isAssistantCollapsed ? (
                                    <Markdown>{formattedAssistantContent}</Markdown>
                                ) : (
                                    <div>
                                        {formattedAssistantContent}
                                        {assistantNeedsCollapse && isAssistantCollapsed && AssistantCollapseButton}
                                    </div>
                                )}
                                {assistantNeedsCollapse && !isAssistantCollapsed && AssistantCollapseButton}
                            </div>
                            {tips.length > 0 && (
                                <Typography.Text type="secondary" className="text-xs mt-1 block">
                                    {tips.join(' • ')}
                                </Typography.Text>
                            )}
                        </div>
                    </div>
                </div>
            )}
            
            {/* Divider between messages */}
            <Divider className="my-1" style={{ borderColor: '#f0f0f0' }} />
        </div>
    )
}
