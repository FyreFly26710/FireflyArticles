'use client';

import { useState, useEffect } from 'react';
import MessageList from '@/components/aiChat/MessageList'
import InputBox from '@/components/aiChat/InputBox'
import SessionSidebar from '@/components/aiChat/SessionSidebar'
import ChatSidebar from '@/components/aiChat/ChatSidebar'
import { storage, LayoutSettings } from '@/states/localStorage';

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
    <div className="h-screen flex overflow-hidden bg-white">
      {/* Left Sidebar */}
      <SessionSidebar />

      {/* Main content area */}
      <main
        className="flex-1 flex flex-col bg-white transition-all duration-300"
        style={{
          marginLeft: sidebarWidth,
          marginRight: rightSidebarWidth
        }}
      >
        {/* Chat container - handles positioning of both message list and input */}
        <div
          className="fixed inset-0 bg-white"
          style={{
            top: '64px', // Header height
            left: sidebarWidth,
            right: rightSidebarWidth,
            transition: 'all 0.3s'
          }}
        >
          {/* Flexbox container for vertical layout */}
          <div className="relative h-full flex flex-col">
            {/* Message list container */}
            <div
              className="flex-1 overflow-hidden"
              style={{ paddingBottom: '140px' }} // Make room for input box
            >
              <div className="h-full max-w-4xl mx-auto px-4 w-full">
                <MessageList />
              </div>
            </div>

            {/* Input box container */}
            <div
              className="absolute bottom-0 left-0 right-0 bg-white border-t border-gray-200"
              style={{
                height: 'auto',
                maxHeight: '400px',  // Allow more space for settings panel
                transition: 'all 0.3s'
              }}
            >
              <div className="h-full max-w-4xl mx-auto px-4 w-full">
                <InputBox />
              </div>
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
