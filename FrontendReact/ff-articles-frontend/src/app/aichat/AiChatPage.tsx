'use client';

import { useEffect } from 'react';
import MessageList from '@/components/aiChat/MessageList'
import InputBox from '@/components/aiChat/InputBox'
import SessionSidebar from '@/components/aiChat/SessionSidebar'
import ChatSidebar from '@/components/aiChat/ChatSidebar'
import { useSettings } from '@/hooks/useSettings';
import { useChat } from '@/hooks/useChat';
import styles from './AiChatPage.module.css';

export default function AiChatPage() {
  const { settings, updateLayoutSettings } = useSettings();
  const { layout: layoutSettings } = settings;
  const { initialize } = useChat();

  useEffect(() => {
    initialize();
  }, []);

  const sidebarWidth = layoutSettings.sidebarCollapsed ? '80px' : '240px';
  const rightSidebarWidth = layoutSettings.rightSidebarCollapsed ? '0px' : '240px';

  const handleToggleRightSidebar = () => {
    updateLayoutSettings({
      ...layoutSettings,
      rightSidebarCollapsed: !layoutSettings.rightSidebarCollapsed
    });
  };

  return (
    <div className={styles.container}>
      {/* Left Sidebar */}
      <SessionSidebar />

      {/* Main content area */}
      <main
        className={styles.main}
        style={{
          marginLeft: sidebarWidth,
          marginRight: rightSidebarWidth
        }}
      >
        {/* Chat container - handles positioning of both message list and input */}
        <div
          className={styles.chatContainer}
          style={{
            top: '64px', // Header height
            left: sidebarWidth,
            right: rightSidebarWidth
          }}
        >
          {/* Flexbox container for vertical layout */}
          <div className={styles.flexContainer}>
            {/* Message list container */}
            <div className={styles.messageContainer}>
              <MessageList />
            </div>

            {/* Input box container */}
            <div className={styles.inputContainer}>
              <InputBox />
            </div>
          </div>
        </div>
      </main>

      {/* Right Sidebar */}
      <ChatSidebar
        collapsed={layoutSettings.rightSidebarCollapsed}
        toggleCollapsed={handleToggleRightSidebar}
      />
    </div>
  );
}
