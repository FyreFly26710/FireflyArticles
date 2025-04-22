'use client';

import React from 'react';
import { Alert } from 'antd';

interface EmptyArticleStateProps {
  message?: string;
}

const EmptyArticleState: React.FC<EmptyArticleStateProps> = ({ message }) => {
  return (
    <Alert
      message="No Articles Generated"
      description={message || "No article suggestions were generated. Please try again with a different topic."}
      type="info"
      showIcon
    />
  );
};

export default EmptyArticleState; 