import React, { useEffect, useRef, useState } from 'react';
import { Anchor, Button, Checkbox, Dropdown, message, Modal } from 'antd';
import {
  DoubleRightOutlined,
  DoubleLeftOutlined,
  DeleteOutlined,
  StopOutlined,
  PlayCircleOutlined,
  MoreOutlined,
  SettingOutlined
} from '@ant-design/icons';
import type { MenuProps } from 'antd';
import { useChat } from '@/stores/ChatContext';

interface ChatSidebarProps {
  collapsed: boolean;
  toggleCollapsed: () => void;
}

const ChatSidebar: React.FC<ChatSidebarProps> = ({ collapsed, toggleCollapsed }) => {
  const { session, handleDisableChatRounds, handleEnableChatRounds, handleDeleteChatRounds } = useChat();
  const sidebarContentRef = useRef<HTMLDivElement>(null);
  const [isModifyMode, setIsModifyMode] = useState(false);
  const [selectedChatIds, setSelectedChatIds] = useState<number[]>([]);

  // Actions dropdown items
  const dropdownItems: MenuProps['items'] = [
    {
      key: 'enable',
      label: 'Enable',
      icon: <PlayCircleOutlined />,
      onClick: handleEnableSelected,
    },
    {
      key: 'disable',
      label: 'Disable',
      icon: <StopOutlined />,
      onClick: handleDisableSelected,
    },
    {
      type: 'divider',
    },
    {
      key: 'delete',
      label: 'Delete',
      icon: <DeleteOutlined />,
      danger: true,
      onClick: handleDeleteSelected,
    },
  ];

  // Generate anchor items from chat rounds
  const anchorItems = session?.rounds?.map((round, index) => {
    // Truncate message for display
    const messagePreview = round?.userMessage?.length > 25
      ? round.userMessage.substring(0, 25) + '...'
      : round?.userMessage || 'No message';

    const isDisabled = !round?.isActive;

    if (isModifyMode) {
      // When in modify mode, show checkboxes instead of anchors
      return {
        key: `chat-round-${round?.chatRoundId}`,
        href: '#', // Add dummy href to satisfy type requirements
        title: (
          <div className="flex items-center">
            <Checkbox
              checked={selectedChatIds.includes(round?.chatRoundId)}
              onChange={(e) => {
                if (e.target.checked) {
                  setSelectedChatIds(prev => [...prev, round?.chatRoundId]);
                } else {
                  setSelectedChatIds(prev => prev.filter(id => id !== round?.chatRoundId));
                }
              }}
            />
            <div className="flex flex-col ml-2">
              <span className={`font-semibold ${isDisabled ? 'line-through text-gray-400' : ''}`}>
                {`Chat ${index + 1}`}
                {isDisabled && <span className="ml-1 text-xs text-gray-400">(inactive)</span>}
              </span>
              <span className="text-xs text-gray-500 truncate">
                {messagePreview}
              </span>
            </div>
          </div>
        ),
      };
    } else {
      // Standard mode with anchors
      return {
        key: `chat-round-${round?.chatRoundId}`,
        href: `#chat-round-${round?.chatRoundId}`,
        title: (
          <div className="flex flex-col">
            <span className={`font-semibold ${isDisabled ? 'line-through text-gray-400' : ''}`}>
              {`Chat ${index + 1}`}
              {isDisabled && <span className="ml-1 text-xs text-gray-400">(inactive)</span>}
            </span>
            <span className="text-xs text-gray-500 truncate">
              {messagePreview}
            </span>
          </div>
        ),
      };
    }
  }) || [];

  // Function to scroll to the latest message
  const scrollToLatest = () => {
    const container = document.querySelector('.message-list-container');
    if (container) {
      container.scrollTop = container.scrollHeight;
    }
  };

  // Function to scroll the sidebar to the bottom
  const scrollSidebarToBottom = () => {
    if (sidebarContentRef.current) {
      sidebarContentRef.current.scrollTop = sidebarContentRef.current.scrollHeight;
    }
  };

  // Exit modify mode and clear selections
  const exitModifyMode = () => {
    setIsModifyMode(false);
    setSelectedChatIds([]);
  };

  // Handle enabling selected chat rounds
  function handleEnableSelected() {
    if (selectedChatIds.length === 0) {
      message.warning('Please select at least one chat to enable');
      return;
    }

    handleEnableChatRounds(selectedChatIds)
      .then(() => {
        message.success(`${selectedChatIds.length} chat(s) enabled`);
        exitModifyMode();
      })
      .catch(error => {
        message.error('Failed to enable chats');
        console.error(error);
      });
  }

  // Handle disabling selected chat rounds
  function handleDisableSelected() {
    if (selectedChatIds.length === 0) {
      message.warning('Please select at least one chat to disable');
      return;
    }

    handleDisableChatRounds(selectedChatIds)
      .then(() => {
        message.success(`${selectedChatIds.length} chat(s) disabled`);
        exitModifyMode();
      })
      .catch(error => {
        message.error('Failed to disable chats');
        console.error(error);
      });
  }

  // Handle deleting selected chat rounds
  function handleDeleteSelected() {
    if (selectedChatIds.length === 0) {
      message.warning('Please select at least one chat to delete');
      return;
    }

    Modal.confirm({
      title: 'Delete chats',
      content: `Are you sure you want to delete ${selectedChatIds.length} chat(s)? This action cannot be undone.`,
      okText: 'Delete',
      okType: 'danger',
      cancelText: 'Cancel',
      onOk: () => {
        return handleDeleteChatRounds(selectedChatIds)
          .then(() => {
            message.success(`${selectedChatIds.length} chat(s) deleted`);
            exitModifyMode();
          })
          .catch(error => {
            message.error('Failed to delete chats');
            console.error(error);
          });
      }
    });
  }

  // Toggle modify mode
  const toggleModifyMode = () => {
    if (isModifyMode) {
      exitModifyMode();
    } else {
      setIsModifyMode(true);
      setSelectedChatIds([]);
    }
  };

  // Scroll sidebar to the bottom when component mounts or when new messages arrive
  useEffect(() => {
    scrollSidebarToBottom();
  }, [session?.rounds?.length]);

  return (
    <>
      {/* Toggle button */}
      <Button
        type="text"
        icon={collapsed ? <DoubleLeftOutlined /> : <DoubleRightOutlined />}
        onClick={toggleCollapsed}
        className="fixed z-10 transition-all duration-300"
        style={{
          right: collapsed ? '0px' : '250px',
          top: '74px',
          position: 'fixed'
        }}
      />

      {/* Right sidebar */}
      <div
        className="fixed bg-white border-l border-gray-200 transition-all duration-300 overflow-hidden"
        style={{
          top: '64px', // Header height
          right: collapsed ? '-240px' : '0',
          width: '240px',
          height: 'calc(100vh - 64px)',
          boxShadow: collapsed ? 'none' : '-2px 0 5px rgba(0, 0, 0, 0.05)',
        }}
      >
        <div className="flex flex-col h-full">
          <div className="p-3 border-b border-gray-200 flex justify-between items-center">
            <h3 className="text-sm font-medium text-gray-700">Chats</h3>
            <div className="flex gap-2">
              <Button
                size="small"
                onClick={toggleModifyMode}
                type={isModifyMode ? "primary" : "default"}
              >
                {isModifyMode ? "Done" : "Modify"}
              </Button>
              <Button size="small" onClick={scrollToLatest}>
                Latest
              </Button>
            </div>
          </div>

          {isModifyMode && (
            <div className="p-2 border-b border-gray-200 flex justify-between items-center">
              <span className="text-sm">{selectedChatIds.length} selected</span>
              <Dropdown
                menu={{ items: dropdownItems }}
                trigger={['hover']}
                placement="bottomRight"
                disabled={selectedChatIds.length === 0}
              >
                <Button size="small" icon={<MoreOutlined />} className="hover:bg-gray-100">
                  Actions
                </Button>
              </Dropdown>
            </div>
          )}

          <div
            className="flex-1 overflow-y-auto p-2"
            ref={sidebarContentRef}
          >
            {anchorItems.length > 0 ? (
              <Anchor
                items={anchorItems}
                affix={false}
                onClick={isModifyMode ? undefined : undefined}
                getContainer={() => {
                  // Cast to HTMLElement to satisfy type requirements
                  const container = document.querySelector('.message-list-container');
                  return container as HTMLElement || window;
                }}
              />
            ) : (
              <div className="text-center text-gray-500 mt-4 p-2">
                No chat messages yet
              </div>
            )}
          </div>
        </div>
      </div>
    </>
  );
};

export default ChatSidebar;
