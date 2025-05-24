'use client';

import React from 'react';
import { Form, Input, Button, InputNumber, Card, Typography, Row, Col, Tooltip, AutoComplete, Spin, Radio, Space } from 'antd';
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
            layout="horizontal"
            labelCol={{ span: 4 }}
            wrapperCol={{ span: 20 }}
            onFinish={handleSubmit}
            initialValues={{
              articleCount: 8,
              provider: modelOptions[0].provider
            }}
          >
            {/* Category */}
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

            {/* Topic */}
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

            {/* Description */}
            <Form.Item
              name="topicAbstract"
              label="Description"
            >
              <Input.TextArea rows={2} size="large" style={{ resize: 'none' }} placeholder="Optional: Enter a description for the topic" />
            </Form.Item>

            {/* Model Selection */}
            <Form.Item
              label="AI Model"
              style={{ marginBottom: 0 }}
            >
              <Row gutter={16}>
                <Col span={14}>
                  <Form.Item
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
                </Col>
                <Col span={10}>
                  <Form.Item
                    rules={[{ required: true, message: 'Required' }]}
                  >
                    <Space.Compact>
                      <span style={{
                        display: 'inline-block',
                        lineHeight: '40px',
                        marginRight: '8px',
                        whiteSpace: 'nowrap'
                      }}>
                        Number of Articles:
                      </span>
                      <Form.Item name="articleCount" noStyle>
                        <InputNumber
                          min={1}
                          max={30}
                          style={{ width: 'calc(100% - 140px)' }}
                          size="large"
                        />
                      </Form.Item>
                    </Space.Compact>
                  </Form.Item>
                </Col>
              </Row>
            </Form.Item>

            <Form.Item
              name="userPrompt"
              label="User Prompt"
            >
              <Input.TextArea rows={2} size="large" style={{ resize: 'none' }} placeholder="Optional: Add your own prompt" />
            </Form.Item>

            <Row gutter={24}>
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
          </Form>
        </Col>

        <Col xs={24} md={8}>
          <Card
            size="small"
            style={{ height: '100%', backgroundColor: '#f9f9f9' }}
          >
            <Typography.Title level={5} style={{ marginTop: 0 }}>For New Topics:</Typography.Title>
            <ul style={{ paddingLeft: 16, marginBottom: 16 }}>
              <li>Be specific with your topic name and description</li>
              <li>Include target audience and their expected skill level</li>
              <li>Consider the scope - is it broad enough for multiple articles?</li>
              <li>Add industry context or technical domain information</li>
            </ul>

            <Typography.Title level={5} style={{ marginTop: 0 }}>For Existing Topics:</Typography.Title>
            <ul style={{ paddingLeft: 16 }}>
              <li>Review existing articles in the topic first</li>
              <li>Consider what aspects haven't been covered yet</li>
              <li>Think about different angles or perspectives</li>
              <li>Aim to complement rather than duplicate content</li>
            </ul>

            <Typography.Title level={5} style={{ marginTop: 16 }}>General Tips:</Typography.Title>
            <ul style={{ paddingLeft: 16 }}>
              <li>Use clear, concise language in descriptions</li>
              <li>Higher article counts work better for broad topics</li>
              <li>Add a topic image to improve visibility</li>
              <li>Custom prompts can help guide the AI's focus</li>
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