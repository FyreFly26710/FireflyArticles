'use client';

import React, { useEffect, useState } from 'react';
import { Form, Input, Button, InputNumber, Card, Typography, Row, Col, Tooltip, AutoComplete, Spin, Radio } from 'antd';
import { SendOutlined, InfoCircleOutlined, LoadingOutlined, MessageOutlined } from '@ant-design/icons';
import { apiTopicGetByPage } from '@/api/contents/api/topic';
import { useArticleGeneration } from '@/hooks/useAiGenArticle';
import { useAiGenContext } from '@/states/AiGenContext';
import { getTopicsByCategory } from '@/libs/utils/articleUtils';
import AiPromptDrawer from '@/components/shared/AiPromptDrawer';
const { Title, Paragraph } = Typography;

const ArticleGenerationForm: React.FC = () => {
  const [form] = Form.useForm<API.ArticleListRequest>();
  const { generateArticles } = useArticleGeneration();
  const [categories, setCategories] = useState<string[]>([]);
  const [loadingCategories, setLoadingCategories] = useState(false);
  const [topics, setTopics] = useState<API.TopicDto[]>([]);
  const [topicsByCategory, setTopicsByCategory] = useState<Record<string, API.TopicDto[]>>({});
  const [topicOptions, setTopicOptions] = useState<{ value: string }[]>([]);
  const { loading } = useAiGenContext();

  // Simplified for the drawer - we only need visibility and form data
  const [drawerVisible, setDrawerVisible] = useState(false);
  const [formData, setFormData] = useState<API.ArticleListRequest | null>(null);

  useEffect(() => {
    fetchTopics();
  }, []);

  const fetchTopics = async () => {
    setLoadingCategories(true);
    try {
      const response = await apiTopicGetByPage({
        OnlyCategoryTopic: true,
        PageSize: 100,
      });

      if (response.data?.data) {
        const topicsData = response.data.data;

        // Extract unique categories
        const uniqueCategories = Array.from(
          new Set(
            topicsData
              .map((topic: API.TopicDto) => topic.category)
              .filter(Boolean) as string[]
          )
        ).sort();

        setTopics(topicsData);
        setCategories(uniqueCategories);

        // Use the utility function to organize topics by category
        const topicsByCat = getTopicsByCategory(topicsData);
        setTopicsByCategory(topicsByCat);
      }
    } catch (error) {
      console.error('Failed to fetch topics:', error);
    } finally {
      setLoadingCategories(false);
    }
  };

  const handleCategoryChange = (categoryValue: string) => {
    form.setFieldsValue({ topic: undefined });

    // Update topic options based on selected category
    if (categoryValue && topicsByCategory[categoryValue]) {
      const options = topicsByCategory[categoryValue].map(topic => ({
        value: topic.title || ''
      }));
      setTopicOptions(options);
    } else {
      setTopicOptions([]);
    }
  };

  const handleSubmit = async (values: API.ArticleListRequest) => {
    try {
      await generateArticles(values);
    } catch (error) {
      console.error('Failed to generate articles:', error);
    }
  };

  // Open drawer with form data
  const openPromptDrawer = async () => {
    try {
      const values = await form.validateFields();
      setFormData(values);
      setDrawerVisible(true);
    } catch (error) {
      console.error('Validation failed:', error);
    }
  };

  // Handle prompt confirmation
  const handlePromptConfirm = async () => {
    if (!formData) return;

    try {
      await generateArticles(formData);
      setDrawerVisible(false);
    } catch (error) {
      console.error('Failed to generate articles after prompt confirmation:', error);
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
          onClose={() => setDrawerVisible(false)}
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