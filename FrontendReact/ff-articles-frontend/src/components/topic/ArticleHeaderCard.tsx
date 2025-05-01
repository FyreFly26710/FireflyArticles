"use client";

import { Card, Typography, Tag, Space, Avatar } from 'antd';
import { UserOutlined, CalendarOutlined } from '@ant-design/icons';
import dynamic from 'next/dynamic';

// Dynamically import MdViewer to avoid SSR issues with markdown rendering
const MdViewer = dynamic(() => import('@/components/shared/MdViewer'), {
  ssr: false,
  loading: () => <div className="h-20 bg-gray-100 rounded animate-pulse" />
});

const { Title, Text } = Typography;

interface ArticleHeaderCardProps {
  topic: API.TopicDto;
}

const ArticleHeaderCard = ({ topic }: ArticleHeaderCardProps) => {
  const formatDate = (dateString?: string) => {
    if (!dateString) return 'Unknown date';
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  return (
    <Card className="shadow-sm mb-4">
      <div className="flex flex-col space-y-4">
        {/* Title */}
        <Title level={2}>{topic.title}</Title>
        
        {/* Topic metadata */}
        <Space className="flex items-center text-gray-500">
          {topic.user && (
            <Space>
              <Avatar 
                src={topic.user.userAvatar} 
                icon={!topic.user.userAvatar ? <UserOutlined /> : undefined} 
                size="small" 
              />
              <Text>{topic.user.userName || topic.user.userAccount}</Text>
            </Space>
          )}
          <Space>
            <CalendarOutlined />
            <Text>{formatDate(topic.updateTime || topic.createTime)}</Text>
          </Space>
          {topic.category && (
            <Tag color="blue">{topic.category}</Tag>
          )}
        </Space>
        
        {/* Abstract/Description */}
        {topic.abstract && (
          <div className="mt-4 p-4 bg-gray-50 rounded-md">
            <MdViewer value={topic.abstract} />
          </div>
        )}
      </div>
    </Card>
  );
};

export default ArticleHeaderCard; 