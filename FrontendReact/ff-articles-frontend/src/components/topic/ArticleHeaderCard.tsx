"use client";

import { Card, Typography, Tag, Space, Avatar, Divider } from 'antd';
import { UserOutlined, CalendarOutlined } from '@ant-design/icons';
import dynamic from 'next/dynamic';

// Dynamically import MdViewer to avoid SSR issues with markdown rendering
const MdViewer = dynamic(() => import('@/components/shared/MdViewer'), {
  ssr: false,
  loading: () => <div className="h-20 rounded animate-pulse" />
});

const { Title, Text } = Typography;

interface ArticleHeaderCardProps {
  article: API.ArticleDto;
}

const ArticleHeaderCard = ({ article }: ArticleHeaderCardProps) => {
  const formatDate = (dateString?: string) => {
    if (!dateString) return 'Unknown date';
    return new Date(dateString).toLocaleDateString('en-GB', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  return (
    <Card className="shadow-sm">
      <div className="flex flex-col space-y-4">
        {/* Title */}
        <Title level={2} style={{ marginBottom: 0 }}>
          {article.title}
        </Title>

        {/* Topic metadata */}
        <Space className="flex items-center">
          {article.user && (
            <Space>
              <Avatar
                icon={<UserOutlined />}
                src={article.user.userAvatar}
                size={24}
                style={{ minWidth: 24, minHeight: 24, maxWidth: 24, maxHeight: 24 }}
              />
              <Text>{article.user.userName || article.user.userAccount}</Text>
            </Space>
          )}
          <Divider type="vertical" style={{ height: '20px', margin: '0 8px' }} />

          <Space>
            <CalendarOutlined />
            <Text>{formatDate(article.updateTime || article.createTime)}</Text>
          </Space>

          {article.tags && article.tags.length > 0 && (
            <>
              <Divider type="vertical" style={{ height: '20px', margin: '0 8px' }} />
              <Space size={4}>
                {article.tags.map((tag, index) => (
                  <Tag color="blue" key={index}>{tag}</Tag>
                ))}
              </Space>
            </>
          )}
        </Space>

        {/* Abstract/Description */}
        {article.abstract && (
          <div className="mt-4 rounded-md" style={{ height: 72 }}>
            <MdViewer value={article.abstract} />
          </div>
        )}
      </div>
    </Card>
  );
};

export default ArticleHeaderCard; 