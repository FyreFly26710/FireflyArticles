'use client';

import React, { useState } from 'react';
import { Card, Tag, Typography, Row, Col, Space, Divider, Button, Input, Form, Spin, message } from 'antd';
import { FileTextOutlined, TagsOutlined, EditOutlined, SaveOutlined, CloseOutlined, PlusOutlined, RocketOutlined, LinkOutlined } from '@ant-design/icons';
import Markdown from 'react-markdown';
import styles from '../aigen.module.css';
import Link from 'next/link';
import { useAiGen } from '../context/AiGenContext';
import { EditableArticle } from '../../app/aigen/types';

const { Title, Paragraph } = Typography;
const { TextArea } = Input;

interface ArticleItemProps {
  article: EditableArticle;
  generationStatus?: {
    isGenerating: boolean;
    generatedArticleId?: number;
  };
  topicId: number;
}

const ArticleItem: React.FC<ArticleItemProps> = ({
  article,
  generationStatus,
  topicId
}) => {
  const [tagInput, setTagInput] = useState('');
  const [localGenerating, setLocalGenerating] = useState(false);
  const [localArticleId, setLocalArticleId] = useState<number | undefined>(undefined);

  const {
    handleEditArticle,
    handleSaveArticle,
    handleCancelEdit,
    handleUpdateField,
    handleAddTag,
    handleRemoveTag,
    generateArticleContent
  } = useAiGen();

  const handleTagInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setTagInput(e.target.value);
  };

  const handleAddTagLocal = () => {
    if (!tagInput.trim()) return;
    handleAddTag(article.sortNumber, tagInput.trim());
    setTagInput('');
  };

  const handleGenerateContent = async () => {
    try {
      setLocalGenerating(true);
      const articleId = await generateArticleContent(article);

      if (articleId) {
        setLocalArticleId(articleId);
        message.success('Article content generated successfully!');
      }
    } catch (error) {
      console.error('Error generating article content:', error);
      message.error('Failed to generate article content. Please try again.');
    } finally {
      setLocalGenerating(false);
    }
  };

  // Use either the shared generation status from parent or the local one
  const isGenerating = generationStatus?.isGenerating || localGenerating;
  const generatedArticleId = generationStatus?.generatedArticleId || localArticleId;

  // If article has been generated, show link instead of the card content
  if (generatedArticleId) {
    const articleUrl = `/topic/${topicId}/article/${generatedArticleId}`;
    return (
      <Card className="article-card">
        <div style={{ textAlign: 'center', padding: '20px 0' }}>
          <Title level={5}>Article Generated Successfully!</Title>
          <Paragraph>
            <Link href={articleUrl} target="_blank">
              <Button type="primary" icon={<LinkOutlined />} size="large">
                View Article: {article.title}
              </Button>
            </Link>
          </Paragraph>
        </div>
      </Card>
    );
  }

  // If generating, show loading spinner
  if (isGenerating) {
    const topicUrl = `/topic/${topicId}`;
    return (
      <Card className="article-card">
        <div style={{ textAlign: 'center', padding: '30px 0' }}>
          <Spin size="large" />
          <Paragraph style={{ marginTop: 16 }}>
            Generating article content for &quot;{article.title}&quot;...
          </Paragraph>
          <Paragraph type="secondary">
            This may take a minute or two.
          </Paragraph>
        </div>
      </Card>
    );
  }

  // Regular card with addition of "Generate Content" button
  return (
    <Card
      hoverable={!article.isEditing}
      style={{ width: '100%' }}
      className={`article-card ${article.isEditing ? styles['article-card-editing'] : ''}`}
      onClick={() => !article.isEditing && handleEditArticle(article.sortNumber)}
      actions={article.isEditing ? [
        <Button
          key="save"
          type="primary"
          icon={<SaveOutlined />}
          onClick={(e) => {
            e.stopPropagation();
            handleSaveArticle(article.sortNumber);
          }}
        >
          Save
        </Button>,
        <Button
          key="cancel"
          icon={<CloseOutlined />}
          onClick={(e) => {
            e.stopPropagation();
            handleCancelEdit(article.sortNumber);
          }}
        >
          Cancel
        </Button>
      ] : [
        <Button
          key="edit"
          icon={<EditOutlined />}
          onClick={(e) => {
            e.stopPropagation();
            handleEditArticle(article.sortNumber);
          }}
        >
          Edit
        </Button>,
        <Button
          key="generate"
          type="primary"
          icon={<RocketOutlined />}
          onClick={(e) => {
            e.stopPropagation();
            handleGenerateContent();
          }}
        >
          Generate Content
        </Button>
      ]}
    >
      <Row gutter={[16, 16]}>
        <Col span={24}>
          {article.isEditing ? (
            <Form.Item label="Title" style={{ marginBottom: 8 }}>
              <Input
                value={article.title}
                onChange={(e) => handleUpdateField(article.sortNumber, 'title', e.target.value)}
                onClick={(e) => e.stopPropagation()}
              />
            </Form.Item>
          ) : (
            <Title level={4} style={{ marginTop: 0 }}>
              <FileTextOutlined style={{ marginRight: 8 }} />
              {article.title}
            </Title>
          )}
          <Divider style={{ margin: '4px 0' }} />
        </Col>

        <Col span={24}>
          {article.isEditing ? (
            <Form.Item label="Abstract" style={{ marginBottom: 8 }}>
              <TextArea
                value={article.abstract}
                onChange={(e) => handleUpdateField(article.sortNumber, 'abstract', e.target.value)}
                autoSize={{ minRows: 3, maxRows: 6 }}
                onClick={(e) => e.stopPropagation()}
              />
            </Form.Item>
          ) : (
            <Paragraph style={{ fontSize: '16px' }}>
              <Markdown>{article.abstract}</Markdown>
            </Paragraph>
          )}
        </Col>

        <Col span={24}>
          {article.isEditing ? (
            <Form.Item label="Tags" style={{ marginBottom: 8 }}>
              <Space direction="vertical" style={{ width: '100%' }}>
                <Space wrap>
                  {article.tags.map((tag, index) => (
                    <Tag
                      key={index}
                      color="purple"
                      closable
                      onClose={(e) => {
                        e.preventDefault();
                        handleRemoveTag(article.sortNumber, index);
                      }}
                    >
                      {tag}
                    </Tag>
                  ))}
                </Space>
                <Space>
                  <Input
                    placeholder="Add a tag"
                    value={tagInput}
                    onChange={handleTagInputChange}
                    onPressEnter={handleAddTagLocal}
                    onClick={(e) => e.stopPropagation()}
                  />
                  <Button
                    type="primary"
                    icon={<PlusOutlined />}
                    onClick={(e) => {
                      e.stopPropagation();
                      handleAddTagLocal();
                    }}
                  >
                    Add
                  </Button>
                </Space>
              </Space>
            </Form.Item>
          ) : (
            <Space direction="horizontal">
              <TagsOutlined style={{ color: '#722ed1' }} />
              {article.tags.map((tag, index) => (
                <Tag key={index} color="purple">{tag}</Tag>
              ))}
            </Space>
          )}
        </Col>
      </Row>
    </Card>
  );
};

export default ArticleItem; 