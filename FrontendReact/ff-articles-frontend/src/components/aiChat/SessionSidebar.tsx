import { useRef } from 'react'
import { Button, Badge, Layout, Space } from 'antd'
import {
    PlusCircleOutlined,
    MenuFoldOutlined,
    MenuUnfoldOutlined
} from '@ant-design/icons'
import SessionList from './SessionList'
import { useChat } from '@/hooks/useChat'
import { useSettings } from '@/hooks/useSettings'
const { Sider } = Layout

export default function SessionSidebar() {
    const sessionListRef = useRef<HTMLDivElement>(null)
    const { settings, updateLayoutSettings } = useSettings();
    const {
        handleCreateSession,
        handleSelectSession,
        sessions,
    } = useChat();

    const handleCollapse = (value: boolean) => {
        updateLayoutSettings({
            ...settings.layout,
            sidebarCollapsed: value
        });
    }

    const handleCreateNewSession = () => {
        handleCreateSession();
        if (sessionListRef.current) {
            sessionListRef.current.scrollTo(0, 0);
        }
    }

    return (
        <Sider
            style={{
                position: 'fixed',
                top: 64,
                left: 0,
                height: 'calc(100vh - 64px)',
                zIndex: 49,
                backgroundColor: '#fff',
                borderRight: '1px solid #f0f0f0',
                overflow: 'hidden'
            }}
            width={240}
            collapsed={settings.layout.sidebarCollapsed}
        >
            <div className="flex flex-col h-full">
                <div className="flex items-center justify-between p-2">
                    <h2 className="text-lg font-semibold text-gray-800 ml-4">
                        {!settings.layout.sidebarCollapsed && 'Sessions'}
                    </h2>
                    <Button
                        type="text"
                        icon={<MenuFoldOutlined style={{ fontSize: '20px' }} />}
                        onClick={() => handleCollapse(!settings.layout.sidebarCollapsed)}
                    />
                </div>

                {!settings.layout.sidebarCollapsed && (
                    <div className="flex-1 overflow-y-auto" ref={sessionListRef}>
                        <SessionList />
                    </div>
                )}

                <div className="p-4 border-t border-gray-200">
                    <Button
                        type="primary"
                        icon={<PlusCircleOutlined />}
                        onClick={sessions.find(session => session.sessionId === 0)
                            ? () => handleSelectSession(sessions.find(session => session.sessionId === 0)!)
                            : handleCreateNewSession}
                        style={{ width: '100%', textAlign: 'left', height: 40 }}
                    >
                        {!settings.layout.sidebarCollapsed && 'New Chat'}
                    </Button>
                </div>
            </div>
        </Sider>
    )
}
