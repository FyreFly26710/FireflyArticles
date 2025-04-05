'use client';

import MessageList from '@/components/aiChat/MessageList'
import InputBox from '@/components/aiChat/InputBox'
import Sidebar from '@/components/aiChat/Sidebar'
import { useChat } from './context/ChatContext'

export default function AiChatPage() {
  const { sidebarCollapsed } = useChat();

  return (
    <div className="h-screen w-screen bg-white">
      <Sidebar
        openCopilotWindow={() => console.log('Open copilot window')}
        openAboutWindow={() => console.log('Open about window')}
        setOpenSettingWindow={() => console.log('Open settings window')}
      />

      <div className={`transition-all duration-300 ${sidebarCollapsed ? 'ml-[80px]' : 'ml-[240px]'} h-screen bg-white`}>
        <div className="h-[calc(100vh-80px)] overflow-y-auto bg-white">
          <div className="max-w-4xl mx-auto px-4">
            <MessageList />
          </div>
        </div>

        <div className={`fixed bottom-0 ${sidebarCollapsed ? 'left-[80px]' : 'left-[240px]'} right-0 bg-white`}>
          <div className="max-w-4xl mx-auto px-4">
            <InputBox onOpenSettings={() => console.log("Open settings")} />
          </div>
        </div>
      </div>
    </div>
  );
}
