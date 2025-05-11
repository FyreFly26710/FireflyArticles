'use client';

import { useState, useEffect } from 'react';
import MessageList from '@/components/aiChat/MessageList'
import InputBox from '@/components/aiChat/InputBox'
import SessionSidebar from '@/components/aiChat/SessionSidebar'
import ChatSidebar from '@/components/aiChat/ChatSidebar'
import { storage, LayoutSettings } from '@/states/localStorage';
import styles from './AiChatPage.module.css';

export default function AiChatPage() {
  const [layoutSettings, setLayoutSettings] = useState<LayoutSettings>(() => storage.getLayoutSettings());

  useEffect(() => {
    // Handler for storage changes from other tabs
    const handleStorageChange = (event: StorageEvent) => {
      if (event.key === 'layout-settings') {
        setLayoutSettings(storage.getLayoutSettings());
      }
    };

    // Handler for layout settings changes in current tab
    const handleLayoutChange = (event: CustomEvent<LayoutSettings>) => {
      setLayoutSettings(event.detail);
    };

    window.addEventListener('storage', handleStorageChange);
    window.addEventListener('layoutSettingsChanged', handleLayoutChange);

    return () => {
      window.removeEventListener('storage', handleStorageChange);
      window.removeEventListener('layoutSettingsChanged', handleLayoutChange);
    };
  }, []);

  const sidebarWidth = layoutSettings.sidebarCollapsed ? '80px' : '240px';
  const rightSidebarWidth = layoutSettings.rightSidebarCollapsed ? '0px' : '240px';

  const toggleRightSidebar = () => {
    const newSettings = {
      ...layoutSettings,
      rightSidebarCollapsed: !layoutSettings.rightSidebarCollapsed
    };
    storage.setLayoutSettings(newSettings);
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
        toggleCollapsed={toggleRightSidebar}
      />
    </div>
  );
}
