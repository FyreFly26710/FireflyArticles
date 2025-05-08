'use client';

import React from 'react';
import Link from 'next/link';
import { Card, Typography, Tag, Button, Space, Spin, Tooltip, message } from 'antd';
import { FileTextOutlined, ArrowRightOutlined, CheckCircleOutlined, LoadingOutlined } from '@ant-design/icons';
import { useAiGenContext } from '@/states/AiGenContext';
import { useAiGenEdit } from '@/hooks/useAiGenEditArticle';

const { Paragraph } = Typography;

interface ArticleCardProps {
    article: API.AIGenArticleDto;
}

const ArticleCard: React.FC<ArticleCardProps> = ({ article }) => {
    const { generationState, parsedArticles } = useAiGenContext();
    const { generateContent } = useAiGenEdit();

    const articleState = generationState[article.sortNumber] || { loading: false };
    const isGenerated = typeof articleState.articleId === 'number';
    const isLoading = articleState.loading;
    const hasError = !!articleState.error;

    const handleGenerateContent = () => {
        // Prevent duplicate generation attempts
        if (isLoading) {
            message.info('This article is already being generated');
            return;
        }

        if (isGenerated) {
            message.info('This article has already been generated');
            return;
        }

        generateContent(article);
    };

    const getArticleUrl = () => {
        if (!parsedArticles || !isGenerated) return '#';
        return `/topic/${parsedArticles.topicId}/article/${articleState.articleId}`;
    };

    return (
        <Card
            title={`#${article.sortNumber}: ${article.title}`}
            style={{ height: '100%' }}
            extra={
                isGenerated && (
                    <Tooltip title="View Article">
                        <Link href={getArticleUrl()} target="_blank">
                            <FileTextOutlined style={{ fontSize: '18px', cursor: 'pointer' }} />
                        </Link>
                    </Tooltip>
                )
            }
        >
            <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
                <Paragraph>{article.abstract}</Paragraph>

                <div style={{ marginTop: '12px', marginBottom: '16px' }}>
                    {article.tags.map((tag, index) => (
                        <Tag key={index} color="blue">
                            {tag}
                        </Tag>
                    ))}
                </div>

                <div style={{ marginTop: 'auto' }}>
                    {isGenerated ? (
                        <Link href={getArticleUrl()} target="_blank">
                            <div>
                                <CheckCircleOutlined /> Go to: {article.title}
                            </div>
                        </Link>
                    ) : isLoading ? (
                        <Button
                            type="primary"
                            loading
                            disabled
                        >
                            Generating...
                        </Button>
                    ) : hasError ? (
                        <Tooltip title={articleState.error}>
                            <Button
                                danger
                                onClick={handleGenerateContent}
                            >
                                Failed - Retry
                            </Button>
                        </Tooltip>
                    ) : (
                        <Button
                            type="primary"
                            icon={<ArrowRightOutlined />}
                            onClick={handleGenerateContent}
                        >
                            Generate Content
                        </Button>
                    )}
                </div>
            </div>
        </Card>
    );
};

export default ArticleCard; 