'use client';

import React, { useEffect, useState } from 'react';
import { Form, Input, Button, InputNumber, Card, Typography, Row, Col, Tooltip, AutoComplete, Spin, Radio } from 'antd';
import { SendOutlined, InfoCircleOutlined, LoadingOutlined } from '@ant-design/icons';
import { apiTopicGetByPage } from '@/api/contents/api/topic';
import { useArticleGeneration } from '@/hooks/useAiGenArticle';
import { useAiGenContext } from '@/states/AiGenContext';
const { Title, Paragraph } = Typography;

const ArticleGenerationForm: React.FC = () => {
  const [form] = Form.useForm<API.ArticleListRequest>();
  const { generateArticles } = useArticleGeneration();
  const [categories, setCategories] = useState<string[]>([]);
  const [loadingCategories, setLoadingCategories] = useState(false);
  const { loading } = useAiGenContext();

  useEffect(() => {
    fetchCategories();
  }, []);

  const fetchCategories = async () => {
    setLoadingCategories(true);
    try {
      const response = await apiTopicGetByPage({
        OnlyCategoryTopic: true,
        PageSize: 100,
      });

      if (response.data?.data) {
        // Extract unique categories
        const uniqueCategories = Array.from(
          new Set(
            response.data.data
              .map((topic: API.TopicDto) => topic.category)
              .filter(Boolean) as string[]
          )
        ).sort();

        setCategories(uniqueCategories);
      }
    } catch (error) {
      console.error('Failed to fetch categories:', error);
    } finally {
      setLoadingCategories(false);
    }
  };

  const handleSubmit = async (values: API.ArticleListRequest) => {
    try {
      await generateArticles(values);
    } catch (error) {
      console.error('Failed to generate articles:', error);
    }
  };

  const modelOptions = [
    { label: 'DeepSeek-V3', provider: 'deepseek' },
    { label: 'Gemini 2.5', provider: 'gemini' }
  ];

  const handleModelChange = (e: any) => {
    const selectedModel = modelOptions.find(model => model.provider === e.target.value);
    if (selectedModel) {
      form.setFieldsValue({
        provider: selectedModel.provider
      });
    }
  };

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
                      Category {loadingCategories && <Spin indicator={<LoadingOutlined style={{ marginLeft: 8 }} spin />} />}
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
                  >
                    <Input size="large" />
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
              <Input size="large" />
            </Form.Item>

            {/* Description - Full Width, 2 lines */}
            <Form.Item
              name="topicAbstract"
              label="Description"
              rules={[{ required: true, message: 'Please enter topic description' }]}
            >
              <Input.TextArea rows={2} size="large" style={{ resize: 'none' }} />
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

            <Form.Item>
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
    </div>
  );
};

export default ArticleGenerationForm; 