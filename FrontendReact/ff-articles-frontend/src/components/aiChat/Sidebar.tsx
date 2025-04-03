import { useRef, useState } from 'react'
import { Button, Badge, Layout, Space } from 'antd'
import {
    SettingOutlined,
    InfoCircleOutlined,
    RobotOutlined,
    PlusCircleOutlined,
    MenuFoldOutlined,
    MenuUnfoldOutlined
} from '@ant-design/icons'
import SessionList from './SessionList'
import { Session } from '@/types/chat'

const { Sider } = Layout

interface SidebarProps {
    openCopilotWindow: () => void
    openAboutWindow: () => void
    setOpenSettingWindow: (name: 'ai' | 'display' | null) => void
    sessions: Session[]
    currentSessionId: string
    onSelectSession: (session: Session) => void
    onEditSession: (session: Session) => void
    onDeleteSession: (session: Session) => void
    onCreateSession: () => void
    onCollapse: (collapsed: boolean) => void
    className?: string
}

export default function Sidebar({
    openCopilotWindow,
    openAboutWindow,
    setOpenSettingWindow,
    sessions,
    currentSessionId,
    onSelectSession,
    onEditSession,
    onDeleteSession,
    onCreateSession,
    onCollapse,
    className
}: SidebarProps) {
    const sessionListRef = useRef<HTMLDivElement>(null)
    const [collapsed, setCollapsed] = useState(false)

    const handleCollapse = (value: boolean) => {
        setCollapsed(value)
        onCollapse(value)
    }

    const handleCreateNewSession = () => {
        onCreateSession()
        if (sessionListRef.current) {
            sessionListRef.current.scrollTo(0, 0)
        }
    }

    return (
        <Sider
            style={{
                position: 'fixed',
                top: 44,
                left: 0,
                height: 'calc(100vh - 40px)',
                zIndex: 50,
                backgroundColor: '#fff',
                borderRight: '1px solid #f0f0f0'
            }}
            width={240}
            collapsed={collapsed}
            className={className}
        >
            <div className="flex flex-col h-full">
                <div className="flex items-center justify-end p-2">
                    <Button
                        type="text"
                        icon={
                            collapsed ? (
                                <MenuUnfoldOutlined style={{ fontSize: '20px' }} />
                            ) : (
                                <MenuFoldOutlined style={{ fontSize: '20px' }} />
                            )
                        }
                        onClick={() => handleCollapse(!collapsed)}
                    />

                </div>

                {!collapsed && (
                    <div className="flex-1 overflow-y-auto">
                        <SessionList
                            sessions={sessions}
                            currentSessionId={currentSessionId}
                            onSelectSession={onSelectSession}
                            onEditSession={onEditSession}
                            onDeleteSession={onDeleteSession}
                            onCreateSession={onCreateSession}
                        />
                    </div>
                )}

                <div className="p-4 border-t border-gray-200">
                    <Space direction="vertical" style={{ width: '100%' }}>
                        <Button
                            type="primary"
                            icon={<PlusCircleOutlined />}
                            onClick={handleCreateNewSession}
                            style={{ width: '100%', textAlign: 'left', height: 40 }}
                        >
                            {!collapsed && 'New Chat'}
                        </Button>

                        <Button
                            type="text"
                            icon={<RobotOutlined />}
                            onClick={openCopilotWindow}
                            style={{ width: '100%', textAlign: 'left', height: 40 }}
                        >
                            {!collapsed && 'My Copilots'}
                        </Button>

                        <Button
                            type="text"
                            icon={<SettingOutlined />}
                            onClick={() => setOpenSettingWindow('ai')}
                            style={{ width: '100%', textAlign: 'left', height: 40 }}
                        >
                            {!collapsed && 'Settings'}
                        </Button>

                        <Button
                            type="text"
                            icon={
                                <Badge dot>
                                    <InfoCircleOutlined />
                                </Badge>
                            }
                            onClick={openAboutWindow}
                            style={{ width: '100%', textAlign: 'left', height: 40 }}
                        >
                            {!collapsed && 'About'}
                        </Button>
                    </Space>
                </div>
            </div>
        </Sider>
    )
}
