'use client';

import React, { useState, useEffect, useRef, useCallback } from 'react';
import { Card, Tag, Typography, List, Row, Col, Space, Divider, Alert, Button, Input, Form } from 'antd';
import { FileTextOutlined, TagsOutlined, CopyOutlined, EditOutlined, SaveOutlined, CloseOutlined, PlusOutlined } from '@ant-design/icons';
import Markdown from 'react-markdown';
import styles from '../aigen.module.css';

const { Title, Paragraph, Text } = Typography;
const { TextArea } = Input;

interface ArticleResultsProps {
  results: API.ArticlesAIResponseDto['data'];
  onUpdateResults?: (updatedResults: API.ArticlesAIResponseDto['data']) => void;
}

interface EditableArticle extends API.AIGenArticleDto {
  isEditing: boolean;
}

const ArticleResults: React.FC<ArticleResultsProps> = ({ results, onUpdateResults }) => {
  const [editableArticles, setEditableArticles] = useState<EditableArticle[]>(() => {
    if (!results || !results.articles) return [];
    return results.articles.map(article => ({
      ...article,
      isEditing: false
    }));
  });
  
  const [tagInputs, setTagInputs] = useState<{ [key: number]: string }>({});
  const initialRender = useRef(true);
  const shouldUpdateParent = useRef(false);
  const lastSavedArticles = useRef<API.AIGenArticleDto[]>([]);

  // Update editableArticles when results change (from parent)
  useEffect(() => {
    if (results && results.articles) {
      // Don't reset editing state if only parent is updating from our own changes
      if (shouldUpdateParent.current) {
        shouldUpdateParent.current = false;
        return;
      }
      
      setEditableArticles(results.articles.map(article => ({
        ...article,
        isEditing: false
      })));
    }
  }, [results]);

  // Only notify parent when explicitly saving changes, not on every keystroke
  const saveToParent = useCallback(() => {
    if (!onUpdateResults || !results) return;
    
    const currentArticles = editableArticles.map(({ isEditing, ...rest }) => rest);
    
    // Update parent with the edited articles
    const updatedResults = {
      ...results,
      articles: currentArticles
    };
    
    shouldUpdateParent.current = true;
    lastSavedArticles.current = currentArticles;
    onUpdateResults(updatedResults);
  }, [editableArticles, onUpdateResults, results]);

  // Handle when component unmounts - save any pending changes
  useEffect(() => {
    return () => {
      if (onUpdateResults && results) {
        const currentArticles = editableArticles.map(({ isEditing, ...rest }) => rest);
        const updatedResults = {
          ...results,
          articles: currentArticles
        };
        onUpdateResults(updatedResults);
      }
    };
  }, []);

  if (!results || !results.articles || results.articles.length === 0) {
    return (
      <Alert
        message="No Articles Generated"
        description="No article suggestions were generated. Please try again with a different topic."
        type="info"
        showIcon
      />
    );
  }

  const handleCopyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text);
  };

  const handleEditArticle = (articleId: number) => {
    setEditableArticles(prevArticles => 
      prevArticles.map(article => 
        article.id === articleId 
          ? { ...article, isEditing: true } 
          : article
      )
    );
  };

  const handleSaveArticle = (articleId: number) => {
    setEditableArticles(prevArticles => {
      const newArticles = prevArticles.map(article => 
        article.id === articleId 
          ? { ...article, isEditing: false } 
          : article
      );
      
      // After updating the state, save to parent
      setTimeout(() => saveToParent(), 0);
      
      return newArticles;
    });
  };

  const handleCancelEdit = (articleId: number) => {
    setEditableArticles(prevArticles => 
      prevArticles.map(article => {
        if (article.id === articleId) {
          // Find the original article from results to restore its data
          const originalArticle = results.articles.find(a => a.id === articleId);
          return {
            ...originalArticle!,
            isEditing: false
          };
        }
        return article;
      })
    );
  };

  const updateArticleField = (articleId: number, field: keyof API.AIGenArticleDto, value: any) => {
    // We need to ensure this doesn't trigger our parent update
    setEditableArticles(prevArticles => 
      prevArticles.map(article => 
        article.id === articleId 
          ? { ...article, [field]: value } 
          : article
      )
    );
  };

  const handleTagInputChange = (articleId: number, value: string) => {
    setTagInputs(prev => ({ ...prev, [articleId]: value }));
  };

  const handleAddTag = (articleId: number) => {
    const tag = tagInputs[articleId]?.trim();
    if (!tag) return;
    
    setEditableArticles(prevArticles => 
      prevArticles.map(article => {
        if (article.id === articleId) {
          const updatedTags = [...article.tags, tag];
          return { ...article, tags: updatedTags };
        }
        return article;
      })
    );
    
    // Clear the tag input
    setTagInputs(prev => ({ ...prev, [articleId]: '' }));
  };

  const handleRemoveTag = (articleId: number, tagIndex: number) => {
    setEditableArticles(prevArticles => 
      prevArticles.map(article => {
        if (article.id === articleId) {
          const updatedTags = [...article.tags];
          updatedTags.splice(tagIndex, 1);
          return { ...article, tags: updatedTags };
        }
        return article;
      })
    );
  };

  return (
    <div>
      {results.aiMessage && (
        <Alert
          message="AI Insights"
          description={results.aiMessage}
          type="success"
          showIcon
          style={{ marginBottom: 24 }}
        />
      )}

      <Card 
        title={
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Title level={4} style={{ margin: 0 }}>Generated Article Suggestions</Title>
            <Text type="secondary">{editableArticles.length} articles generated</Text>
          </div>
        }
        bordered={false}
        className="article-results-card"
        extra={
          <Button
            icon={<CopyOutlined />}
            onClick={() => handleCopyToClipboard(
              editableArticles.map(a => `# ${a.title}\n\n${a.abstract}\n\nTags: ${a.tags.join(', ')}`).join('\n\n---\n\n')
            )}
          >
            Copy All
          </Button>
        }
      >
        <List
          itemLayout="vertical"
          dataSource={editableArticles}
          renderItem={(article) => (
            <List.Item key={article.id}>
              <Card 
                hoverable={!article.isEditing}
                style={{ width: '100%' }}
                className={`article-card ${article.isEditing ? styles['article-card-editing'] : ''}`}
                onClick={() => !article.isEditing && handleEditArticle(article.id)}
                actions={article.isEditing ? [
                  <Button 
                    key="save" 
                    type="primary" 
                    icon={<SaveOutlined />} 
                    onClick={(e) => {
                      e.stopPropagation();
                      handleSaveArticle(article.id);
                    }}
                  >
                    Save
                  </Button>,
                  <Button 
                    key="cancel" 
                    icon={<CloseOutlined />} 
                    onClick={(e) => {
                      e.stopPropagation();
                      handleCancelEdit(article.id);
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
                      handleEditArticle(article.id);
                    }}
                  >
                    Edit
                  </Button>,
                  <Button 
                    key="copy" 
                    icon={<CopyOutlined />} 
                    onClick={(e) => {
                      e.stopPropagation();
                      handleCopyToClipboard(`# ${article.title}\n\n${article.abstract}\n\nTags: ${article.tags.join(', ')}`);
                    }}
                  >
                    Copy
                  </Button>
                ]}
              >
                <Row gutter={[16, 16]}>
                  <Col span={24}>
                    {article.isEditing ? (
                      <Form.Item label="Title" style={{ marginBottom: 8 }}>
                        <Input 
                          value={article.title}
                          onChange={(e) => updateArticleField(article.id, 'title', e.target.value)}
                          onClick={(e) => e.stopPropagation()}
                        />
                      </Form.Item>
                    ) : (
                      <Title level={4} style={{ marginTop: 0 }}>
                        <FileTextOutlined style={{ marginRight: 8 }} />
                        {article.title}
                      </Title>
                    )}
                    <Divider style={{ margin: '4px 0' }}/>
                  </Col>
                  
                  <Col span={24}>
                    {article.isEditing ? (
                      <Form.Item label="Abstract" style={{ marginBottom: 8 }}>
                        <TextArea
                          value={article.abstract}
                          onChange={(e) => updateArticleField(article.id, 'abstract', e.target.value)}
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
                                  handleRemoveTag(article.id, index);
                                }}
                              >
                                {tag}
                              </Tag>
                            ))}
                          </Space>
                          <Space>
                            <Input
                              placeholder="Add a tag"
                              value={tagInputs[article.id] || ''}
                              onChange={(e) => handleTagInputChange(article.id, e.target.value)}
                              onPressEnter={() => handleAddTag(article.id)}
                              onClick={(e) => e.stopPropagation()}
                            />
                            <Button 
                              type="primary" 
                              icon={<PlusOutlined />} 
                              onClick={(e) => {
                                e.stopPropagation();
                                handleAddTag(article.id);
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
            </List.Item>
          )}
        />
      </Card>
    </div>
  );
};

export default ArticleResults; 