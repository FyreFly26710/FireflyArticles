'use client';

import { useState, useEffect, useRef } from 'react';
import { Button, Anchor } from 'antd';
import { MenuFoldOutlined, MenuUnfoldOutlined } from '@ant-design/icons';
import MessageList from '@/components/aiChat/MessageList'
import InputBox from '@/components/aiChat/InputBox'
import SessionSidebar from '@/components/aiChat/SessionSidebar'
import ChatSidebar from '@/components/aiChat/ChatSidebar'
import { useChat } from './context/ChatContext'

export default function AiChatPage() {
  const { 
    sidebarCollapsed, 
    rightSidebarCollapsed, 
    setRightSidebarCollapsed 
  } = useChat();
  
  const sidebarWidth = sidebarCollapsed ? '80px' : '240px';
  const rightSidebarWidth = rightSidebarCollapsed ? '0px' : '240px';

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
        collapsed={rightSidebarCollapsed} 
        toggleCollapsed={() => setRightSidebarCollapsed(!rightSidebarCollapsed)} 
      />
    </div>
  );
}
