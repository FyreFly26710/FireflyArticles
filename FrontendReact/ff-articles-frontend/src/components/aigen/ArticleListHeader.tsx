'use client';

import React from 'react';
import { Typography, Button } from 'antd';
import { RocketOutlined } from '@ant-design/icons';

const { Title, Text } = Typography;

interface ArticleListHeaderProps {
  articleCount: number;
}

const ArticleListHeader: React.FC<ArticleListHeaderProps> = ({ 
  articleCount,
}) => {
  return (
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
      <Title level={4} style={{ margin: 0 }}>Generated Article Suggestions</Title>
      <Text type="secondary">{articleCount} articles generated</Text>
    </div>
  );
};

interface ArticleListActionsProps {
  onGenerateAll?: () => void;
  isGeneratingAll?: boolean;
}

export const ArticleListActions: React.FC<ArticleListActionsProps> = ({ onGenerateAll, isGeneratingAll }) => {
  return (
    <Button 
      icon={<RocketOutlined />} 
      onClick={onGenerateAll}
      type="primary"
      loading={isGeneratingAll}
    >
      Generate All
    </Button>
  );
};

export default ArticleListHeader; 