"use client";

import { FloatButton, message } from 'antd';
import {
  PlusOutlined,
  EditOutlined,
  VerticalAlignTopOutlined,
  ReloadOutlined
} from '@ant-design/icons';
import { useRouter } from 'next/navigation';

interface ArticleButtonsProps {
  article: API.ArticleDto;
  onEditModal: () => void;
  onRegenerateArticle: () => void;
}

const ArticleButtons = ({ article, onEditModal, onRegenerateArticle }: ArticleButtonsProps) => {
  const router = useRouter();

  const handleNewArticle = () => {
    router.push(`/topic/${article.topicId}/new`);
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
        onClick={onEditModal}
      />
      <FloatButton
        icon={<ReloadOutlined />}
        tooltip="Regenerate with AI"
        onClick={onRegenerateArticle}
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