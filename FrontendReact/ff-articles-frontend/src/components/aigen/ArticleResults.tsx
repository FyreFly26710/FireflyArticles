'use client';

import React, { useEffect } from 'react';
import { Card, Alert, Row, Col } from 'antd';
import { useAiGenContext } from '@/states/AiGenContext';
import { useAiGenEdit } from '@/hook/useAiGenEditArticle';
import ArticleRawJson from './ArticleRawJson';
import ArticleCardList from './ArticleGenCardList';

const ArticleResults: React.FC = () => {
  const {
    responseData,
    parsedArticles,
  } = useAiGenContext();
  const { parseArticleData, handleDataChange } = useAiGenEdit();

  // Try to parse data when the component mounts
  useEffect(() => {
    if (responseData) {
      parseArticleData(responseData);
    }
  }, []);

  // Try to parse data when responseData changes
  useEffect(() => {
    if (responseData) {
      parseArticleData(responseData);
    }
  }, [responseData]);

  if (!responseData && !parsedArticles) {
    return null; // Nothing to display yet
  }

  return (
    <Card title="Article Results">
      <Row gutter={[24, 24]}>
        {/* Left side: JSON Response Text Area */}
        <Col xs={24} md={12}>
          <ArticleRawJson
            responseData={responseData}
            onDataChange={handleDataChange}
            onTryParse={() => parseArticleData(responseData)}
          />
        </Col>

        {/* Right side: Parsed Data Display*/}
        <Col xs={24} md={12}>

          {/* Parsed Articles */}
          {parsedArticles && (
            <ArticleCardList parsedArticles={parsedArticles} />
          )}
        </Col>
      </Row>
    </Card>
  );
};

export default ArticleResults; 