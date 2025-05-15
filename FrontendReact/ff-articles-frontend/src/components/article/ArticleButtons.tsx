"use client";

import { FloatButton, message } from 'antd';
import {
  PlusOutlined,
  EditOutlined,
  VerticalAlignTopOutlined,
  ReloadOutlined
} from '@ant-design/icons';
import { useRouter } from 'next/navigation';
import { apiAiArticlesRegenerateContent } from '@/api/ai/api/aiarticles';

interface ArticleButtonsProps {
  article: API.ArticleDto;
  onEditModal?: () => void;
}

const ArticleButtons = ({ article, onEditModal }: ArticleButtonsProps) => {
  const router = useRouter();

  const handleNewArticle = () => {
    router.push(`/topic/${article.topicId}/new`);
  };

  const handleOpenArticleModal = () => {
    if (onEditModal) {
      onEditModal();
    }
  };

  const handleScrollToTop = () => {
    const contentContainer = document.querySelector('.content-container');
    if (contentContainer) {
      contentContainer.scrollTo({
        top: 0,
        behavior: 'smooth'
      });
    }
  };

  const handleRegenerateArticle = async () => {
    try {

      await apiAiArticlesRegenerateContent({
        articleId: article.articleId,
      });
      // Wait a bit and then refresh the page to show the updated content
      setTimeout(() => {
        window.location.reload();
      }, 1500);
    } catch (error) {
      message.error({ content: 'Failed to regenerate article content', key: 'regenerate' });
      console.error('Error regenerating article:', error);
    }
  };

  return (
    <FloatButton.Group
      trigger="hover"
      style={{ right: 24, bottom: 24 }}
      shape="square"
      icon={<PlusOutlined />}
    >
      <FloatButton
        icon={<PlusOutlined />}
        tooltip="New Article"
        onClick={handleNewArticle}
      />
      <FloatButton
        icon={<EditOutlined />}
        tooltip="Edit Article"
        onClick={handleOpenArticleModal}
      />
      <FloatButton
        icon={<ReloadOutlined />}
        tooltip="Regenerate with AI"
        onClick={handleRegenerateArticle}
      />
      <FloatButton
        icon={<VerticalAlignTopOutlined />}
        tooltip="Back to Top"
        onClick={handleScrollToTop}
      />
    </FloatButton.Group>
  );
};

export default ArticleButtons; 