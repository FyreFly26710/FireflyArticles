"use client";

import { Card, Typography, Tag, Space, Avatar, Divider, Button, Input, Tooltip, message } from 'antd';
import { UserOutlined, CalendarOutlined, EditOutlined, PlusOutlined, CloseOutlined, SaveOutlined, ReloadOutlined } from '@ant-design/icons';
import dynamic from 'next/dynamic';
import { useArticleEdit } from '@/hook/useArticleEdit';
import { useState, useEffect } from 'react';
import TagSelect from '@/components/shared/TagSelect';
import { apiTagGetAll } from '@/api/contents/api/tag';

// Dynamically import MdViewer to avoid SSR issues with markdown rendering
const MdViewer = dynamic(() => import('@/components/shared/MdViewer'), {
  ssr: false,
  loading: () => <div className="h-20 rounded animate-pulse" />
});

const { Title, Text } = Typography;
const { TextArea } = Input;

interface ArticleHeaderCardProps {
  article: API.ArticleDto;
  onRegenerateArticle: () => void;
}

const ArticleHeaderCard = ({ article, onRegenerateArticle }: ArticleHeaderCardProps) => {
  const { isEditing, currentArticle, startEditing, updateArticle, cancelEditing, submitEdit } = useArticleEdit();
  const [tags, setTags] = useState<API.TagDto[]>([]);
  useEffect(() => {
    const fetchTags = async () => {
      try {
        const response = await apiTagGetAll();
        if (response.data) {
          setTags(response.data);
        }
      } catch (error) {
        setTags([]);
      }
    };
    if (isEditing) {
      fetchTags();
    }
  }, [isEditing]);

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-GB', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  const displayArticle = isEditing ? currentArticle : article;

  if (!displayArticle) return null;

  const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    updateArticle({ title: e.target.value });
  };

  const handleAbstractChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    updateArticle({ abstract: e.target.value });
  };

  return (
    <Card className="shadow-sm">
      <div className="flex flex-col space-y-4">
        {/* Title with edit/save/cancel buttons */}
        <div className="flex justify-between items-center">
          {isEditing ? (
            <Input
              value={displayArticle.title}
              onChange={handleTitleChange}
              placeholder="Article title"
              className="text-2xl font-bold"
              style={{ maxWidth: 'calc(100% - 190px)' }}
            />
          ) : (
            <Title level={2} style={{ marginBottom: 0 }}>
              {displayArticle.title}
            </Title>
          )}

          {isEditing ? (
            <Space>
              <Button
                icon={<CloseOutlined />}
                onClick={cancelEditing}
                danger
              >
                Cancel
              </Button>
              <Button
                type="primary"
                icon={<SaveOutlined />}
                onClick={submitEdit}
              >
                Save
              </Button>
            </Space>
          ) : (
            <Space.Compact>
              <Button
                type="primary"
                icon={<EditOutlined />}
                onClick={() => startEditing(article)}
              >
                Edit
              </Button>
              <Button
                type="primary"
                icon={<ReloadOutlined />}
                onClick={onRegenerateArticle}
              >
                Generate
              </Button>
            </Space.Compact>
          )}
        </div>

        {/* Topic metadata */}
        <Space className="flex items-center">
          {displayArticle.user && (
            <Space>
              <Avatar
                icon={<UserOutlined />}
                src={displayArticle.user.userAvatar}
                size={24}
                style={{ minWidth: 24, minHeight: 24, maxWidth: 24, maxHeight: 24 }}
              />
              <Text>{displayArticle.user.userName || displayArticle.user.userAccount}</Text>
            </Space>
          )}
          <Divider type="vertical" style={{ height: '20px', margin: '0 8px' }} />
          {displayArticle.updateTime && displayArticle.createTime && (
            <Space>
              <CalendarOutlined />
              <Text>{formatDate(displayArticle.updateTime || displayArticle.createTime)}</Text>
            </Space>
          )}

          {/* Tags - editable when in edit mode */}
          <Divider type="vertical" style={{ height: '20px', margin: '0 8px' }} />
          {isEditing ? (
            <div style={{ flex: 1, maxWidth: 600 }}>
              <TagSelect
                tags={tags}
                selectedTags={displayArticle.tags || []}
                onChange={(newTags) => updateArticle({ tags: newTags })}
              />
            </div>
          ) : (
            <Space size={4} wrap>
              {displayArticle.tags && displayArticle.tags.map((tag, index) => (
                <Tag color="blue" key={index}>{tag}</Tag>
              ))}
            </Space>
          )}
        </Space>

        {/* Abstract/Description - use TextArea in edit mode */}
        {isEditing ? (
          <div className="mt-4">
            <TextArea
              value={displayArticle.abstract || ''}
              onChange={handleAbstractChange}
              placeholder="Enter article abstract..."
              autoSize={{ minRows: 3, maxRows: 6 }}
            />
          </div>
        ) : displayArticle.abstract ? (
          <div className="mt-4 rounded-md" style={{ height: 72 }}>
            <MdViewer value={displayArticle.abstract} />
          </div>
        ) : null}
      </div>
    </Card>
  );
};

export default ArticleHeaderCard; 