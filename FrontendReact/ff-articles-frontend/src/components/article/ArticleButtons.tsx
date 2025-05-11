"use client";

import { FloatButton } from 'antd';
import {
  PlusOutlined,
  EditOutlined,
  VerticalAlignTopOutlined
} from '@ant-design/icons';
import { useRouter } from 'next/navigation';

interface ArticleButtonsProps {
  topicId?: number;
  onEditModal?: () => void;
}

const ArticleButtons = ({ topicId, onEditModal }: ArticleButtonsProps) => {
  const router = useRouter();

  const handleNewArticle = () => {
    if (topicId) {
      router.push(`/topic/${topicId}/new`);
    }
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

  // Don't show buttons if no topicId
  if (!topicId) return null;

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
        icon={<VerticalAlignTopOutlined />}
        tooltip="Back to Top"
        onClick={handleScrollToTop}
      />
    </FloatButton.Group>
  );
};

export default ArticleButtons; 