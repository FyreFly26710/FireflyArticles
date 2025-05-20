"use client";

import { Card } from 'antd';
import dynamic from 'next/dynamic';
import { useArticleEdit } from '@/hook/useArticleEdit';

// Dynamically import MdViewer to avoid SSR issues with markdown rendering
const MdViewer = dynamic(() => import('@/components/shared/MdViewer'), {
  ssr: false,
  loading: () => <div className="h-96 bg-gray-100 rounded animate-pulse" />
});

// Dynamically import MdEditor for editing mode
const MdEditor = dynamic(() => import('@/components/shared/MdEditor'), {
  ssr: false,
  loading: () => <div className="h-96 bg-gray-100 rounded animate-pulse" />
});

interface ArticleContentCardProps {
  article: API.ArticleDto;
}

const ArticleContentCard = ({ article }: ArticleContentCardProps) => {
  const { isEditing, currentArticle, updateArticle } = useArticleEdit();

  const displayArticle = isEditing ? currentArticle : article;

  if (!displayArticle) return null;

  const handleContentChange = (value: string) => {
    updateArticle({ content: value });
  };

  if (isEditing) {
    return (
      <Card className="shadow-sm">
        <div className="markdown-editor" style={{ minHeight: 500 }}>
          <MdEditor
            value={displayArticle.content || ''}
            onChange={handleContentChange}
            placeholder="Write your article content here..."
          />
        </div>
      </Card>
    );
  }

  if (!displayArticle.content) {
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
        <MdViewer value={displayArticle.content} />
      </div>
    </Card>
  );
};

export default ArticleContentCard; 