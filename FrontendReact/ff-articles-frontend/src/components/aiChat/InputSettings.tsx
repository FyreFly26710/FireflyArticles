'use client'
import React, { useEffect, useState } from 'react';
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
import { storage, ChatDisplaySettings, ChatBehaviorSettings, SelectedModel } from '@/stores/storage';
import { apiAiAssistantProviders } from '@/api/ai/api/assistant';

const { Text } = Typography;
const { Option } = Select;

interface InputSettingsProps {
  visible: boolean;
}

const InputSettings: React.FC<InputSettingsProps> = ({ visible }) => {
  // Initialize with current settings
  const [displaySettings, setDisplaySettings] = useState(() => storage.getChatDisplaySettings());
  const [behaviorSettings, setBehaviorSettings] = useState(() => storage.getChatBehaviorSettings());
  const [providers, setProviders] = useState<API.ChatProvider[]>([]);
  const [selectedModel, setSelectedModel] = useState<SelectedModel>(behaviorSettings.selectedModel);
  const [isLoading, setIsLoading] = useState(false);

  // Fetch providers on mount
  useEffect(() => {
    const loadProviders = async () => {
      setIsLoading(true);
      try {
        // Check storage first
        const storedProviders = storage.getChatProviders();
        if (storedProviders) {
          setProviders(storedProviders);
          setIsLoading(false);
          return;
        }

        // Fetch from API if not in storage
        const response = await apiAiAssistantProviders();
        if (response?.data && Array.isArray(response.data)) {
          setProviders(response.data);
          // Save to storage for future use
          storage.setChatProviders(response.data);
        }
      } catch (error) {
        console.error('Error loading providers:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadProviders();
  }, []);

  // Listen for settings changes
  useEffect(() => {
    const handleDisplayChange = (event: CustomEvent<ChatDisplaySettings>) => {
      setDisplaySettings(event.detail);
    };

    const handleBehaviorChange = (event: CustomEvent<ChatBehaviorSettings>) => {
      setBehaviorSettings(event.detail);
    };

    const handleSelectedModelChange = (event: CustomEvent<SelectedModel>) => {
      setSelectedModel(event.detail);
    };

    window.addEventListener('chatDisplaySettingsChanged', handleDisplayChange);
    window.addEventListener('chatBehaviorSettingsChanged', handleBehaviorChange);
    window.addEventListener('selectedModelChanged', handleSelectedModelChange);
    return () => {
      window.removeEventListener('chatDisplaySettingsChanged', handleDisplayChange);
      window.removeEventListener('chatBehaviorSettingsChanged', handleBehaviorChange);
      window.removeEventListener('selectedModelChanged', handleSelectedModelChange);
    };
  }, []);

  // Update display settings
  const updateDisplaySetting = (key: keyof ChatDisplaySettings, value: boolean) => {
    const newSettings = { ...displaySettings, [key]: value };
    storage.setChatDisplaySettings(newSettings);
  };

  // Update behavior settings
  const updateBehaviorSetting = (key: keyof ChatBehaviorSettings, value: any) => {
    const newSettings = { ...behaviorSettings, [key]: value };
    storage.setChatBehaviorSettings(newSettings);
  };

  const updateSelectedModel = (value: SelectedModel) => {
    storage.setSelectedModel(value);
  };

  // Create a unique key for each model option
  const getModelKey = (provider: string, model: string): string => {
    return `${provider}|${model}`;
  };


  return (
    <div
      className={`transition-all duration-300 overflow-hidden ${visible ? 'opacity-100 max-h-[220px]' : 'opacity-0 max-h-0'
        }`}
    >
      <div className="rounded-lg border border-gray-200 bg-white shadow-sm p-3">
        <div className="flex flex-wrap gap-4 items-center justify-between mb-2">
          {/* Left column - model selection */}
          <div className="flex items-center gap-2">
            <Text strong><RobotOutlined /> Model:</Text>
            <Select
              value={selectedModel.model}
              labelInValue={false}
              optionLabelProp="label"
              onChange={(value) => {
                // Parse the string value into a SelectedModel object
                const [providerName, model] = value.split('|');
                if (providerName && model) {
                  updateSelectedModel({ providerName, model });
                }
              }}
              placeholder="Select Model"
              style={{ width: 180 }}
              dropdownStyle={{ maxHeight: '300px', overflow: 'auto' }}
              listHeight={300}
              loading={isLoading}
              disabled={isLoading || providers.length === 0}
            >
              {providers.map(provider => (
                provider.models.map(model => (
                  <Option
                    key={getModelKey(provider.providerName, model)}
                    value={getModelKey(provider.providerName, model)}
                    label={model}
                  >
                    <div>
                      <div className="font-medium">{model}</div>
                      <div className="text-xs text-gray-500">{provider.providerName}</div>
                    </div>
                  </Option>
                ))
              ))}
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
