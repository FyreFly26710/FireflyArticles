"use client";

import { Card, Typography, Tag, Space, Avatar, Divider, Button, Input, Tooltip, message } from 'antd';
import { UserOutlined, CalendarOutlined, EditOutlined, PlusOutlined, CloseOutlined, SaveOutlined, ReloadOutlined } from '@ant-design/icons';
import dynamic from 'next/dynamic';
import { useArticleEdit } from '@/hooks/useArticleEdit';
import { useState, useEffect } from 'react';
import TagSelect from '@/components/shared/TagSelect';
import { apiTagGetAll } from '@/api/contents/api/tag';
import styled from 'styled-components';

// Styled Components
const StyledButtonGroup = styled.div`
  display: none;
  @media (min-width: 768px) {
    display: flex;
  }
`;

const StyledContainer = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;

  @media (min-width: 768px) {
    flex-direction: row;
    align-items: center;
    gap: 0;
  }
`;

const StyledSection = styled.div`
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
`;

const StyledTagSection = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  
  @media (min-width: 768px) {
    &::before {
      content: '';
      width: 1px;
      height: 20px;
      background-color: rgba(0, 0, 0, 0.06);
      margin: 0 8px;
    }
  }
`;

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
  const {
    isEditing,
    currentArticle,
    tags,
    formatDate,
    startEditing,
    updateArticle,
    cancelEditing,
    submitEdit
  } = useArticleEdit();

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
            <Title level={3} style={{ marginBottom: 0 }}>
              {displayArticle.title}
            </Title>
          )}

          {isEditing ? (
            <StyledButtonGroup>
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
            </StyledButtonGroup>
          ) : (
            <StyledButtonGroup>
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
            </StyledButtonGroup>
          )}
        </div>

        {/* Topic metadata */}
        <StyledContainer>
          <StyledSection>
            {displayArticle.user && (
              <Space>
                <Avatar
                  icon={<UserOutlined />}
                  src={displayArticle.user.userAvatar}
                  size={24}
                  style={{ minWidth: 24, minHeight: 24, maxWidth: 24, maxHeight: 24 }}
                />
                <Text style={{ whiteSpace: 'nowrap' }}>{displayArticle.user.userName || displayArticle.user.userAccount}</Text>
              </Space>
            )}
            {displayArticle.updateTime && displayArticle.createTime && (
              <>
                <Divider type="vertical" style={{ height: '20px', margin: '0' }} />
                <Space>
                  <CalendarOutlined />
                  <Text style={{ whiteSpace: 'nowrap' }}>{formatDate(displayArticle.updateTime || displayArticle.createTime)}</Text>
                </Space>
              </>
            )}
          </StyledSection>

          {/* Tags - editable when in edit mode */}
          <StyledTagSection>
            {isEditing ? (
              <div style={{ flex: 1, maxWidth: 600 }}>
                <TagSelect
                  tags={tags}
                  selectedTags={displayArticle.tags || []}
                  onChange={(newTags) => updateArticle({ tags: newTags })}
                />
              </div>
            ) : (
              displayArticle.tags && displayArticle.tags.map((tag, index) => (
                <Tag color="blue" key={index}>{tag}</Tag>
              ))
            )}
          </StyledTagSection>
        </StyledContainer>

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