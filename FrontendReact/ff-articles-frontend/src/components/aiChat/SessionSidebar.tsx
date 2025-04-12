import { useRef, useState, useEffect } from 'react'
import { Button, Badge, Layout, Space } from 'antd'
import {
    PlusCircleOutlined,
    MenuFoldOutlined,
    MenuUnfoldOutlined
} from '@ant-design/icons'
import SessionList from './SessionList'
import { useChat } from '@/app/aichat/context/ChatContext'
import { storage, LayoutSettings } from '@/stores/storage'
const { Sider } = Layout

export default function SessionSidebar() {
    const sessionListRef = useRef<HTMLDivElement>(null)
    const [layoutSettings, setLayoutSettings] = useState(() => storage.getLayoutSettings());
    const {
        handleCreateSession,
        sessions,
        setSession
    } = useChat();

    // Listen for layout settings changes
    useEffect(() => {
        const handleLayoutChange = (event: CustomEvent<LayoutSettings>) => {
            setLayoutSettings(event.detail);
        };

        const handleStorageChange = (event: StorageEvent) => {
            if (event.key === 'layout-settings') {
                setLayoutSettings(storage.getLayoutSettings());
            }
        };

        window.addEventListener('layoutSettingsChanged', handleLayoutChange);
        window.addEventListener('storage', handleStorageChange);

        return () => {
            window.removeEventListener('layoutSettingsChanged', handleLayoutChange);
            window.removeEventListener('storage', handleStorageChange);
        };
    }, []);

    const handleCollapse = (value: boolean) => {
        const newSettings = { ...layoutSettings, sidebarCollapsed: value };
        storage.setLayoutSettings(newSettings);
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
            collapsed={layoutSettings.sidebarCollapsed}
        >
            <div className="flex flex-col h-full">
                <div className="flex items-center justify-between p-2">
                    <h2 className="text-lg font-semibold text-gray-800 ml-4">
                        {!layoutSettings.sidebarCollapsed && 'Sessions'}
                    </h2>
                    <Button
                        type="text"
                        icon={<MenuFoldOutlined style={{ fontSize: '20px' }} />}
                        onClick={() => handleCollapse(!layoutSettings.sidebarCollapsed)}
                    />
                </div>

                {!layoutSettings.sidebarCollapsed && (
                    <div className="flex-1 overflow-y-auto" ref={sessionListRef}>
                        <SessionList />
                    </div>
                )}

                <div className="p-4 border-t border-gray-200">
                    <Button
                        type="primary"
                        icon={<PlusCircleOutlined />}
                        onClick={sessions.find(session => session.sessionId === 0) 
                            ? () => setSession(sessions.find(session => session.sessionId === 0)!)
                            : handleCreateNewSession}
                        style={{ width: '100%', textAlign: 'left', height: 40 }}
                    >
                        {!layoutSettings.sidebarCollapsed && 'New Chat'}
                    </Button>
                </div>
            </div>
        </Sider>
    )
}
