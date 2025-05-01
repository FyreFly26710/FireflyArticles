"use client";

import { Card } from 'antd';
import dynamic from 'next/dynamic';

// Dynamically import MdViewer to avoid SSR issues with markdown rendering
const MdViewer = dynamic(() => import('@/components/shared/MdViewer'), {
  ssr: false,
  loading: () => <div className="h-96 bg-gray-100 rounded animate-pulse" />
});

interface ArticleContentCardProps {
  topic: API.TopicDto;
}

const ArticleContentCard = ({ topic }: ArticleContentCardProps) => {
  if (!topic.content) {
    return (
      <Card className="shadow-sm">
        <div className="text-center text-gray-400 p-10">
          No content available for this topic.
        </div>
      </Card>
    );
  }

  return (
    <Card className="shadow-sm">
      <div className="markdown-content">
        <MdViewer value={topic.content} />
      </div>
    </Card>
  );
};

export default ArticleContentCard; 