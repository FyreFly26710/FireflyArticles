import React, { useState, useEffect } from 'react';
import { Drawer, Button, Typography, Space, Divider, Spin } from 'antd';
import { MessageOutlined, CheckCircleOutlined, LoadingOutlined } from '@ant-design/icons';
import { apiAiArticlesPromptsGenerateList, apiAiArticlesPromptsRegenerateList, apiAiArticlesPromptsGenerateContent, apiAiArticlesPromptsRegenerateContent } from '@/api/ai/api/aiarticlesprompts';

const { Title, Text } = Typography;

export type PromptType = 'generateList' | 'regenerateList' | 'generateContent' | 'regenerateContent';

interface AiPromptDrawerProps {
  visible: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title?: string;
  width?: number;
  promptType: PromptType;
  requestData: any;
}

const AiPromptDrawer: React.FC<AiPromptDrawerProps> = ({
  visible,
  onClose,
  onConfirm,
  title = 'AI Generated Prompts',
  width = 500,
  promptType,
  requestData,
}) => {
  const [messages, setMessages] = useState<API.MessageDto[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (visible) {
      fetchPrompts();
    }
  }, [visible, promptType, requestData]);

  const fetchPrompts = async () => {
    if (!visible) return;

    setLoading(true);
    try {
      let response: API.MessageDtoListApiResponse | undefined;

      switch (promptType) {
        case 'generateList':
          response = await apiAiArticlesPromptsGenerateList(requestData as API.ArticleListRequest);
          break;
        case 'regenerateList':
          response = await apiAiArticlesPromptsRegenerateList(requestData as API.ExistingArticleListRequest);
          break;
        case 'generateContent':
          response = await apiAiArticlesPromptsGenerateContent(requestData as API.ContentRequest);
          break;
        case 'regenerateContent':
          response = await apiAiArticlesPromptsRegenerateContent(requestData as API.RegenerateArticleContentRequest);
          break;
      }

      if (response?.data) {
        setMessages(response.data);
      }
    } catch (error) {
      console.error(`Failed to fetch ${promptType} prompts:`, error);
    } finally {
      setLoading(false);
    }
  };

  const handleConfirm = () => {
    onConfirm();
    onClose();
  };

  const renderMessageContent = (content: string, role: string) => {
    // Special styling for system messages
    const isSystem = role.toLowerCase() === 'system';

    return (
      <div
        style={{
          backgroundColor: isSystem ? '#f6f6f6' : (role.toLowerCase() === 'user' ? '#e6f7ff' : '#f0f9eb'),
          padding: '12px',
          borderRadius: '8px',
          marginBottom: '12px',
          maxHeight: '500px',
          overflowY: 'auto',
        }}
      >
        <Text strong style={{ color: isSystem ? '#666' : (role.toLowerCase() === 'user' ? '#1890ff' : '#52c41a') }}>
          {role.charAt(0).toUpperCase() + role.slice(1)} Prompt:
        </Text>
        <div style={{ whiteSpace: 'pre-wrap' }}>{content}</div>
      </div>
    );
  };

  return (
    <Drawer
      title={
        <Space>
          <MessageOutlined />
          <span>{title}</span>
        </Space>
      }
      placement="right"
      onClose={onClose}
      open={visible}
      width={width}
      footer={
        <div style={{ textAlign: 'right' }}>
          <Button onClick={onClose} style={{ marginRight: 8 }}>
            Cancel
          </Button>
          <Button onClick={handleConfirm} type="primary" icon={<CheckCircleOutlined />} disabled={loading}>
            Confirm
          </Button>
        </div>
      }
    >
      {loading ? (
        <div style={{ textAlign: 'center', padding: '40px 0' }}>
          <Spin indicator={<LoadingOutlined style={{ fontSize: 24 }} spin />} />
          <div style={{ marginTop: 16 }}>Loading prompts...</div>
        </div>
      ) : (
        <div className="ai-prompt-messages">
          {messages.length === 0 ? (
            <Text type="secondary">No messages to display</Text>
          ) : (
            messages.map((message, index) => (
              <div key={index}>
                {renderMessageContent(message.content, message.role)}
              </div>
            ))
          )}
        </div>
      )}
    </Drawer>
  );
};

export default AiPromptDrawer;
