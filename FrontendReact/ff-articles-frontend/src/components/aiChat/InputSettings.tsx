import React from 'react';
import { Select, Switch, Divider, Typography } from 'antd';
import { 
  ThunderboltOutlined, 
  BulbOutlined,
  EyeOutlined,
  FileTextOutlined,
  ClockCircleOutlined,
  CodeOutlined,
  RobotOutlined,
  CompressOutlined
} from '@ant-design/icons';
import { useChat } from '@/app/aichat/context/ChatContext';
import { storage, ChatSettings } from '@/stores/storage';

const { Text } = Typography;
const { Option } = Select;

// Default settings
const defaultSettings: ChatSettings = {
    showMessageTimestamp: true,
    showModelName: true,
    showTokenUsed: true,
    showTimeTaken: true,
    enableMarkdownRendering: true,
    enableThinking: true,
    enableStreaming: true,
    showOnlyActiveMessages: false,
    enableCollapsibleMessages: true,
    selectedModel: 'deepseek'
};

interface InputSettingsProps {
  visible: boolean;
}

const InputSettings: React.FC<InputSettingsProps> = ({ visible }) => {
  const { 
    showMessageTimestamp,
    showModelName,
    showTokenUsed,
    showTimeTaken,
    enableMarkdownRendering,
    enableThinking,
    enableStreaming,
    showOnlyActiveMessages,
    enableCollapsibleMessages,
    selectedModel
  } = useChat();

  const updateSetting = (key: keyof ChatSettings, value: boolean | string) => {
    const currentSettings = storage.getChatSettings() || defaultSettings;
    const newSettings = { ...currentSettings, [key]: value };
    storage.setChatSettings(newSettings);
  };

  return (
    <div 
      className={`transition-all duration-300 overflow-hidden ${
        visible ? 'opacity-100 max-h-[220px]' : 'opacity-0 max-h-0'
      }`}
    >
      <div className="rounded-lg border border-gray-200 bg-white shadow-sm p-3">
        <div className="flex flex-wrap gap-4 items-center justify-between mb-2">
          {/* Left column - model selection */}
          <div className="flex items-center gap-2">
            <Text strong><RobotOutlined /> Model:</Text>
            <Select 
              value={selectedModel} 
              onChange={(value) => updateSetting('selectedModel', value)}
              placeholder="Select Model"
              style={{ width: 180 }}
              disabled={true}
            >
              <Option value="deepseek">DeepSeek</Option>
              <Option value="gpt-3.5-turbo">GPT-3.5</Option>
              <Option value="claude">Claude</Option>
            </Select>
          </div>
          
          {/* Middle column - features */}
          <div className="flex flex-wrap gap-4">
            <div className="flex items-center gap-2">
              <Switch 
                checked={enableThinking} 
                onChange={(checked) => updateSetting('enableThinking', checked)}
                size="small" 
              />
              <Text><BulbOutlined /> Thinking</Text>
            </div>
            <div className="flex items-center gap-2">
              <Switch 
                checked={enableStreaming} 
                onChange={(checked) => updateSetting('enableStreaming', checked)}
                size="small" 
              />
              <Text><ThunderboltOutlined /> Streaming</Text>
            </div>
            <div className="flex items-center gap-2">
              <Switch 
                checked={showOnlyActiveMessages} 
                onChange={(checked) => updateSetting('showOnlyActiveMessages', checked)}
                size="small"
              />
              <Text><EyeOutlined /> Only Active</Text>
            </div>
            <div className="flex items-center gap-2">
              <Switch 
                checked={enableCollapsibleMessages} 
                onChange={(checked) => updateSetting('enableCollapsibleMessages', checked)}
                size="small"
              />
              <Text><CompressOutlined /> Collapsible</Text>
            </div>
          </div>
        </div>

        <Divider className="my-2" />

        {/* Message Display Settings */}
        <div className="flex flex-wrap gap-4">
          <div className="flex items-center gap-2">
            <Switch 
              checked={showTokenUsed} 
              onChange={(checked) => updateSetting('showTokenUsed', checked)}
              size="small" 
            />
            <Text><FileTextOutlined /> Tokens</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch 
              checked={showModelName} 
              onChange={(checked) => updateSetting('showModelName', checked)}
              size="small" 
            />
            <Text><RobotOutlined /> Model</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch 
              checked={showTimeTaken} 
              onChange={(checked) => updateSetting('showTimeTaken', checked)}
              size="small" 
            />
            <Text><ClockCircleOutlined /> Time</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch 
              checked={showMessageTimestamp} 
              onChange={(checked) => updateSetting('showMessageTimestamp', checked)}
              size="small" 
            />
            <Text><ClockCircleOutlined /> Date</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch 
              checked={enableMarkdownRendering} 
              onChange={(checked) => updateSetting('enableMarkdownRendering', checked)}
              size="small" 
            />
            <Text><CodeOutlined /> Markdown</Text>
          </div>
        </div>
      </div>
    </div>
  );
};

export default InputSettings;
