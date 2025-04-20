'use client';

import React from 'react';
import { Alert } from 'antd';

interface AiInsightProps {
  message: string;
}

const AiInsight: React.FC<AiInsightProps> = ({ message }) => {
  if (!message) return null;
  
  return (
    <Alert
      message="AI Insights"
      description={message}
      type="success"
      showIcon
      style={{ marginBottom: 24 }}
    />
  );
};

export default AiInsight; 