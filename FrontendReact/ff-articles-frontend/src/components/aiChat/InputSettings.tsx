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
import { useSettings } from '@/hooks/useSettings';
import { useChat } from '@/hooks/useChat';

const { Text } = Typography;
const { Option } = Select;

interface InputSettingsProps {
  visible: boolean;
}

const InputSettings: React.FC<InputSettingsProps> = ({ visible }) => {
  const { settings, updateDisplaySettings, updateBehaviorSettings, updateSelectedModel, loadProviders } = useSettings();
  const { providers } = useChat();

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
              value={settings.chatBehavior.selectedModel.model}
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
              loading={!providers}
              disabled={!providers}
            >
              {providers?.map(provider => (
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
                checked={settings.chatBehavior.enableThinking}
                onChange={(checked) => updateBehaviorSettings({
                  ...settings.chatBehavior,
                  enableThinking: checked
                })}
                size="small"
              />
              <Text><BulbOutlined /> Thinking</Text>
            </div>
            <div className="flex items-center gap-2">
              <Switch
                checked={settings.chatBehavior.enableStreaming}
                onChange={(checked) => updateBehaviorSettings({
                  ...settings.chatBehavior,
                  enableStreaming: checked
                })}
                size="small"
              />
              <Text><ThunderboltOutlined /> Streaming</Text>
            </div>
            <div className="flex items-center gap-2">
              <Switch
                checked={settings.chatDisplay.showOnlyActiveMessages}
                onChange={(checked) => updateDisplaySettings({
                  ...settings.chatDisplay,
                  showOnlyActiveMessages: checked
                })}
                size="small"
              />
              <Text><EyeOutlined /> Only Active</Text>
            </div>
            <div className="flex items-center gap-2">
              <Switch
                checked={settings.chatDisplay.enableCollapsibleMessages}
                onChange={(checked) => updateDisplaySettings({
                  ...settings.chatDisplay,
                  enableCollapsibleMessages: checked
                })}
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
              checked={settings.chatDisplay.showTokenUsed}
              onChange={(checked) => updateDisplaySettings({
                ...settings.chatDisplay,
                showTokenUsed: checked
              })}
              size="small"
            />
            <Text><FileTextOutlined /> Tokens</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch
              checked={settings.chatDisplay.showModelName}
              onChange={(checked) => updateDisplaySettings({
                ...settings.chatDisplay,
                showModelName: checked
              })}
              size="small"
            />
            <Text><RobotOutlined /> Model</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch
              checked={settings.chatDisplay.showTimeTaken}
              onChange={(checked) => updateDisplaySettings({
                ...settings.chatDisplay,
                showTimeTaken: checked
              })}
              size="small"
            />
            <Text><ClockCircleOutlined /> Time</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch
              checked={settings.chatDisplay.showMessageTimestamp}
              onChange={(checked) => updateDisplaySettings({
                ...settings.chatDisplay,
                showMessageTimestamp: checked
              })}
              size="small"
            />
            <Text><ClockCircleOutlined /> Date</Text>
          </div>
          <div className="flex items-center gap-2">
            <Switch
              checked={settings.chatDisplay.enableMarkdownRendering}
              onChange={(checked) => updateDisplaySettings({
                ...settings.chatDisplay,
                enableMarkdownRendering: checked
              })}
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
