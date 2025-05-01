"use client";

import { Card } from 'antd';
import dynamic from 'next/dynamic';

// Dynamically import MdViewer to avoid SSR issues with markdown rendering
const MdViewer = dynamic(() => import('@/components/shared/MdViewer'), {
  ssr: false,
  loading: () => <div className="h-96 bg-gray-100 rounded animate-pulse" />
});

interface ArticleContentCardProps {
  article: API.ArticleDto;
}

const ArticleContentCard = ({ article }: ArticleContentCardProps) => {
  if (!article.content) {
    return (
      <Card className="shadow-sm">
        <div className="text-center text-gray-400 p-10">
          No content available for this article.
        </div>
      </Card>
    );
  }

  return (
    <Card className="shadow-sm">
      <div className="markdown-content">
        <MdViewer value={article.content} />
      </div>
    </Card>
  );
};

export default ArticleContentCard; 