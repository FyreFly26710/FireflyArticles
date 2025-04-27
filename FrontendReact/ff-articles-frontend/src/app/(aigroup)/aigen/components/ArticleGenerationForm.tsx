'use client';

import React from 'react';
import { Form, Input, Button, InputNumber, Card, Typography, Row, Col, Tooltip } from 'antd';
import { SendOutlined, InfoCircleOutlined } from '@ant-design/icons';
import { useAiGen } from '../context/AiGenContext';

const { Title, Paragraph } = Typography;

const ArticleGenerationForm: React.FC = () => {
  const [form] = Form.useForm();
  const { generateArticles, loading } = useAiGen();

  const handleSubmit = (values: { topic: string; count: number; category: string }) => {
    generateArticles({ topic: values.topic, category: values.category, articleCount: values.count });
  };

  return (
    <div>
      <Row gutter={[16, 24]}>
        <Col xs={24} md={16}>
          <Paragraph>
            Enter a topic and the number of articles you&apos;d like to generate. Our AI will create
            article suggestions with titles, abstracts, and relevant tags.
          </Paragraph>
          
          <Form 
            form={form}
            layout="vertical"
            onFinish={handleSubmit}
            initialValues={{ count: 3 }}
          >
            <Form.Item
              name="category"
              label="Category"
              rules={[{ required: true, message: 'Please enter a category' }]}
              tooltip="Enter a specific category for which you want to generate articles"
            >
              <Input 
                placeholder="e.g., Technology, Health, Business" 
                size="large"
                suffix={
                  <Tooltip title="Be specific with your category for better results">
                    <InfoCircleOutlined style={{ color: 'rgba(0,0,0,.45)' }} />
                  </Tooltip>
                }
              />
            </Form.Item>
            
            <Form.Item
              name="topic"
              label="Topic"
              rules={[{ required: true, message: 'Please enter a topic' }]}
              tooltip="Enter a specific topic or subject area for which you want to generate articles"
            >
              <Input 
                placeholder="e.g., Artificial Intelligence in Healthcare" 
                size="large"
                suffix={
                  <Tooltip title="Be specific with your topic for better results">
                    <InfoCircleOutlined style={{ color: 'rgba(0,0,0,.45)' }} />
                  </Tooltip>
                }
              />
            </Form.Item>

            <Form.Item
              name="count"
              label="Number of Articles"
              rules={[{ required: true, message: 'Please enter the number of articles' }]}
              tooltip="How many article suggestions do you want to generate (1-10)"
            >
              <InputNumber 
                min={1} 
                max={10} 
                style={{ width: '100%' }} 
                size="large"
              />
            </Form.Item>

            <Form.Item>
              <Button 
                type="primary" 
                htmlType="submit" 
                size="large" 
                loading={loading}
                icon={<SendOutlined />}
                style={{ width: '100%', marginTop: 16 }}
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