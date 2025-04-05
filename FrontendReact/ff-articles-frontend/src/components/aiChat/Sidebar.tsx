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
import { useChat } from '@/app/aichat/context/ChatContext'

const { Sider } = Layout

interface SidebarProps {
    openCopilotWindow: () => void
    openAboutWindow: () => void
    setOpenSettingWindow: (name: 'ai' | 'display' | null) => void
}

export default function Sidebar({
    openCopilotWindow,
    openAboutWindow,
    setOpenSettingWindow
}: SidebarProps) {
    const sessionListRef = useRef<HTMLDivElement>(null)
    const [collapsed, setCollapsed] = useState(false)
    
    const {
        handleCreateSession,
        setSidebarCollapsed
    } = useChat();

    const handleCollapse = (value: boolean) => {
        setCollapsed(value)
        setSidebarCollapsed(value)
    }

    const handleCreateNewSession = () => {
        handleCreateSession()
        if (sessionListRef.current) {
            sessionListRef.current.scrollTo(0, 0)
        }
    }

    return (
        <Sider
            style={{
                position: 'fixed',
                top: 52,
                left: 0,
                height: 'calc(100vh - 40px)',
                zIndex: 50,
                backgroundColor: '#fff',
                borderRight: '1px solid #f0f0f0'
            }}
            width={240}
            collapsed={collapsed}
        >
            <div className="flex flex-col h-full">
                <div className="flex items-center justify-between p-2">
                    <h2 className="text-lg font-semibold text-gray-800 ml-4">
                        {!collapsed && 'Chats'}
                    </h2>
                    <Button
                        type="text"
                        icon={<MenuFoldOutlined style={{ fontSize: '20px' }} />}
                        onClick={() => handleCollapse(!collapsed)}
                    />
                </div>


                {!collapsed && (
                    <div className="flex-1 overflow-y-auto">
                        <SessionList />
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
