'use client'
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
import { storage, ChatDisplaySettings, ChatBehaviorSettings } from '@/stores/storage';

const { Text } = Typography;
const { Option } = Select;

interface InputSettingsProps {
  visible: boolean;
}

const InputSettings: React.FC<InputSettingsProps> = ({ visible }) => {
  // Initialize with current settings
  const [displaySettings, setDisplaySettings] = React.useState(() => storage.getChatDisplaySettings());
  const [behaviorSettings, setBehaviorSettings] = React.useState(() => storage.getChatBehaviorSettings());

  // Listen for settings changes
  React.useEffect(() => {
    const handleDisplayChange = (event: CustomEvent<ChatDisplaySettings>) => {
      setDisplaySettings(event.detail);
    };

    const handleBehaviorChange = (event: CustomEvent<ChatBehaviorSettings>) => {
      setBehaviorSettings(event.detail);
    };

    window.addEventListener('chatDisplaySettingsChanged', handleDisplayChange);
    window.addEventListener('chatBehaviorSettingsChanged', handleBehaviorChange);

    return () => {
      window.removeEventListener('chatDisplaySettingsChanged', handleDisplayChange);
      window.removeEventListener('chatBehaviorSettingsChanged', handleBehaviorChange);
    };
  }, []);

  // Update display settings
  const updateDisplaySetting = (key: keyof ChatDisplaySettings, value: boolean) => {
    const newSettings = { ...displaySettings, [key]: value };
    storage.setChatDisplaySettings(newSettings);
  };

  // Update behavior settings
  const updateBehaviorSetting = (key: keyof ChatBehaviorSettings, value: boolean | string) => {
    const newSettings = { ...behaviorSettings, [key]: value };
    storage.setChatBehaviorSettings(newSettings);
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
              value={behaviorSettings.selectedModel} 
              onChange={(value) => updateBehaviorSetting('selectedModel', value)}
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
                checked={behaviorSettings.enableThinking} 
                onChange={(checked) => updateBehaviorSetting('enableThinking', checked)}
                size="small" 
              />
              <Text><BulbOutlined /> Thinking</Text>
            </div>
            <div className="flex items-center gap-2">
              <Switch 
                checked={behaviorSettings.enableStreaming} 
                onChange={(checked) => updateBehaviorSetting('enableStreaming', checked)}
                size="small" 
              />
              <Text><ThunderboltOutlined /> Streaming</Text>
            </div>
            <div className="flex items-center gap-2">
              <Switch 
                checked={displaySettings.showOnlyActiveMessages} 
                onChange={(checked) => updateDisplaySetting('showOnlyActiveMessages', checked)}
                size="small"
              />
              <Text><EyeOutlined /> Only Active</Text>
            </div>
            <div className="flex items-center gap-2">
              <Switch 
                checked={displaySettings.enableCollapsibleMessages} 
                onChange={(checked) => updateDisplaySetting('enableCollapsibleMessages', checked)}
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
              checked={displaySettings.showTokenUsed} 
              onChange={(checked) => updateDisplaySetting('showTokenUsed', checked)}
              size="small" 
            />
            <Text><FileTextOutlined /> Tokens</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch 
              checked={displaySettings.showModelName} 
              onChange={(checked) => updateDisplaySetting('showModelName', checked)}
              size="small" 
            />
            <Text><RobotOutlined /> Model</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch 
              checked={displaySettings.showTimeTaken} 
              onChange={(checked) => updateDisplaySetting('showTimeTaken', checked)}
              size="small" 
            />
            <Text><ClockCircleOutlined /> Time</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch 
              checked={displaySettings.showMessageTimestamp} 
              onChange={(checked) => updateDisplaySetting('showMessageTimestamp', checked)}
              size="small" 
            />
            <Text><ClockCircleOutlined /> Date</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch 
              checked={displaySettings.enableMarkdownRendering} 
              onChange={(checked) => updateDisplaySetting('enableMarkdownRendering', checked)}
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
