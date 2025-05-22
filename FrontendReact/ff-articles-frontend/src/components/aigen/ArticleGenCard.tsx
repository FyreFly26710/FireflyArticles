'use client';

import React, { useState } from 'react';
import Link from 'next/link';
import { Card, Typography, Tag, Button, Space, Tooltip, message } from 'antd';
import { FileTextOutlined, ArrowRightOutlined, CheckCircleOutlined, MessageOutlined } from '@ant-design/icons';
import { useAiGen } from '@/hooks/useAiGen';
import { useAiGenArticleEdit } from '@/hooks/useAiGenArticleEdit';
import AiPromptDrawer from '@/components/shared/AiPromptDrawer';

const { Paragraph } = Typography;

interface ArticleGenCardProps {
    article: API.AIGenArticleDto;
}

const ArticleGenCard: React.FC<ArticleGenCardProps> = ({ article }) => {
    const { generationState, parsedArticles, articleListRequest } = useAiGen();
    const { generateContent } = useAiGenArticleEdit();
    const [promptVisible, setPromptVisible] = useState(false);

    const articleState = generationState[article.sortNumber] || { loading: false };
    const isGenerated = typeof articleState.articleId === 'number';
    const isLoading = articleState.loading;

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

    const handleShowPrompts = () => {
        setPromptVisible(true);
    };

    const handleClosePrompts = () => {
        setPromptVisible(false);
    };

    const handleConfirmPrompts = () => {
        setPromptVisible(false);
        handleGenerateContent();
    };

    const getArticleUrl = () => {
        if (!parsedArticles || !isGenerated) return '#';
        return `/topic/${parsedArticles.topicId}/article/${articleState.articleId}`;
    };

    const getContentRequest = (): API.ContentRequest => ({
        sortNumber: article.sortNumber,
        category: articleListRequest?.category || '',
        title: article.title,
        abstract: article.abstract,
        tags: article.tags,
        topic: articleListRequest?.topic || '',
        topicAbstract: articleListRequest?.topicAbstract || '',
        topicId: parsedArticles?.topicId || 0,
        provider: articleListRequest?.provider || 'DeepSeek'
    });

    return (
        <>
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
                        ) : (
                            <Space>
                                <Button
                                    type="primary"
                                    icon={<ArrowRightOutlined />}
                                    onClick={handleGenerateContent}
                                >
                                    Generate Content
                                </Button>
                                <Button
                                    icon={<MessageOutlined />}
                                    onClick={handleShowPrompts}
                                    style={{ padding: '0px 4px' }}
                                >
                                    Prompts
                                </Button>
                            </Space>
                        )}
                    </div>
                </div>
            </Card>

            <AiPromptDrawer
                visible={promptVisible}
                onClose={handleClosePrompts}
                onConfirm={handleConfirmPrompts}
                title={`AI Prompts - ${article.title}`}
                promptType="generateContent"
                requestData={getContentRequest()}
            />
        </>
    );
};

export default ArticleGenCard; 