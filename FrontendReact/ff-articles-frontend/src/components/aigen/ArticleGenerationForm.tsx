'use client';

import React from 'react';
import { Form, Input, Button, InputNumber, Card, Typography, Row, Col, Tooltip, AutoComplete, Spin, Radio } from 'antd';
import { SendOutlined, MessageOutlined } from '@ant-design/icons';
import { useAiGenForm } from '@/hooks/useAiGenForm';
import AiPromptDrawer from '@/components/shared/AiPromptDrawer';

const { Title, Paragraph } = Typography;

const ArticleGenerationForm: React.FC = () => {
  const {
    form,
    loading,
    loadingCategories,
    categories,
    topicOptions,
    modelOptions,
    formData,
    drawerVisible,
    handleCategoryChange,
    handleSubmit,
    handleModelChange,
    handlePromptConfirm,
    openPromptDrawer,
    closePromptDrawer
  } = useAiGenForm();

  return (
    <div>
      <Paragraph className="mb-4">
        Enter a topic and description for the articles you&apos;d like to generate. Our AI will create
        article suggestions with titles, abstracts, and relevant tags.
      </Paragraph>

      <Row gutter={[16, 24]}>
        <Col xs={24} md={16}>
          <Form
            form={form}
            layout="vertical"
            onFinish={handleSubmit}
            initialValues={{
              articleCount: 5,
              provider: modelOptions[0].provider
            }}
          >
            {/* Row for Category and Number of Articles */}
            <Row gutter={16}>
              <Col xs={24} md={18}>
                <Form.Item
                  name="category"
                  label={
                    <span>
                      Category {loadingCategories && <Spin />}
                    </span>
                  }
                  rules={[{ required: true, message: 'Please enter a category' }]}
                >
                  <AutoComplete
                    options={categories.map(category => ({ value: category }))}
                    size="large"
                    filterOption={(inputValue, option) =>
                      option!.value.toUpperCase().indexOf(inputValue.toUpperCase()) !== -1
                    }
                    onChange={handleCategoryChange}
                  >
                    <Input size="large" placeholder="Please select a category or enter a new one" />
                  </AutoComplete>
                </Form.Item>
              </Col>
              <Col xs={24} md={6}>
                <Form.Item
                  name="articleCount"
                  label="Number of Articles"
                  rules={[{ required: true, message: 'Required' }]}
                >
                  <InputNumber
                    min={1}
                    max={20}
                    style={{ width: '100%' }}
                    size="large"
                  />
                </Form.Item>
              </Col>
            </Row>

            {/* Topic - Full Width */}
            <Form.Item
              name="topic"
              label="Topic"
              rules={[{ required: true, message: 'Please enter a topic' }]}
            >
              <AutoComplete
                options={topicOptions}
                size="large"
                filterOption={(inputValue, option) =>
                  option!.value.toUpperCase().indexOf(inputValue.toUpperCase()) !== -1
                }
              >
                <Input size="large" placeholder="Select an existing topic or enter a new one" />
              </AutoComplete>
            </Form.Item>

            <Form.Item
              name="topicImage"
              label="Topic Image"
            >
              <Input size="large" placeholder="Enter topic image url" />
            </Form.Item>

            {/* Description - Full Width, 2 lines */}
            <Form.Item
              name="topicAbstract"
              label="Description"
            >
              <Input.TextArea rows={2} size="large" style={{ resize: 'none' }} placeholder="Optional: Enter a description for the topic" />
            </Form.Item>

            {/* Model Selection */}
            <Form.Item
              label="AI Model"
              name="provider"
              rules={[{ required: true, message: 'Please select an AI model' }]}
            >
              <Radio.Group onChange={handleModelChange}>
                {modelOptions.map(option => (
                  <Radio.Button key={option.provider} value={option.provider}>
                    {option.label}
                  </Radio.Button>
                ))}
              </Radio.Group>
            </Form.Item>
            <Form.Item name="userPrompt" label="User Prompt">
              <Input.TextArea rows={2} size="large" style={{ resize: 'none' }} placeholder="Optional: Add your own prompt" />
            </Form.Item>

            <Form.Item>
              <Row gutter={16}>
                <Col span={12}>
                  <Button
                    type="default"
                    size="large"
                    icon={<MessageOutlined />}
                    style={{ width: '100%', marginTop: 16 }}
                    onClick={openPromptDrawer}
                    disabled={loading}
                  >
                    View Prompts
                  </Button>
                </Col>
                <Col span={12}>
                  <Button
                    type="primary"
                    htmlType="submit"
                    size="large"
                    icon={<SendOutlined />}
                    style={{ width: '100%', marginTop: 16 }}
                    loading={loading}
                    disabled={loading}
                  >
                    {loading ? 'Generating...' : 'Generate Articles'}
                  </Button>
                </Col>
              </Row>
            </Form.Item>
          </Form>
        </Col>

        <Col xs={24} md={8}>
          <Card
            title="Tips for Better Results"
            size="small"
            style={{ height: '100%', backgroundColor: '#f9f9f9' }}
          >
            <ul style={{ paddingLeft: 16 }}>
              <li>Be specific with your topic</li>
              <li>Include target audience if applicable</li>
              <li>Use clear, concise language</li>
              <li>Consider adding industry or domain context</li>
            </ul>
          </Card>
        </Col>
      </Row>

      {formData && (
        <AiPromptDrawer
          visible={drawerVisible}
          onClose={closePromptDrawer}
          onConfirm={handlePromptConfirm}
          title="AI Prompt Preview"
          width={600}
          promptType="generateList"
          requestData={formData}
        />
      )}
    </div>
  );
};

export default ArticleGenerationForm; 