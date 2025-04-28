'use client';

import React from 'react';
import { Card, List, Alert } from 'antd';
import { useAiGen } from '@/stores/aiGenContext';
import ArticleItem from './ArticleItem';
import AiInsight from './AiInsight';
import ArticleListHeader, { ArticleListActions } from './ArticleListHeader';
import EmptyArticleState from './EmptyArticleState';
import styles from '@/styles/aigroup.module.css';

const ArticleResults: React.FC = () => {
  const { 
    results, 
    editableArticles, 
    generationStatus,
    isGeneratingAll,
    generateAllArticles
  } = useAiGen();

  if (!results || !results.articles || results.articles.length === 0) {
    return <EmptyArticleState message={results?.aiMessage} />;
  }

  return (
    <div>
      <AiInsight message={results.aiMessage} />

      <Card 
        title={<ArticleListHeader articleCount={editableArticles.length} />}
        bordered={false}
        className={styles['article-results-card']}
        extra={
          <ArticleListActions 
            onGenerateAll={generateAllArticles} 
            isGeneratingAll={isGeneratingAll}
          />
        }
      >
        <List
          itemLayout="vertical"
          dataSource={editableArticles}
          renderItem={(article) => (
            <List.Item key={article.sortNumber}>
              <ArticleItem
                article={article}
                generationStatus={generationStatus[article.sortNumber]}
                topicId={results.topicId}
              />
            </List.Item>
          )}
        />
      </Card>
    </div>
  );
};

export default ArticleResults; 