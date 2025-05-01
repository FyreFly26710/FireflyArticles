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
}

const ArticleButtons = ({ topicId }: ArticleButtonsProps) => {
  const router = useRouter();

  const handleNewArticle = () => {
    if (topicId) {
      router.push(`/topic/${topicId}/new`);
    }
  };

  const handleEditTopic = () => {
    if (topicId) {
      router.push(`/topics/edit/${topicId}`);
    }
  };

  const handleScrollToTop = () => {
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  };

  // Don't show buttons if no topicId
  if (!topicId) return null;

  return (
    <FloatButton.Group
      trigger="hover"
      style={{ right: 24, bottom: 24 }}
      icon={<PlusOutlined />}
    >
      <FloatButton 
        icon={<PlusOutlined />} 
        tooltip="New Article" 
        onClick={handleNewArticle} 
      />
      <FloatButton 
        icon={<EditOutlined />} 
        tooltip="Edit Topic" 
        onClick={handleEditTopic} 
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