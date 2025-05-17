"use client";

import { Row, Col, message } from 'antd';
import { useState } from 'react';
import dynamic from 'next/dynamic';
import AiPromptDrawer from '@/components/shared/AiPromptDrawer';
import { apiAiArticlesRegenerateContent } from '@/api/ai/api/aiarticles';
// Dynamically import components to reduce initial bundle size
const ArticleHeaderCard = dynamic(() => import('@/components/article/ArticleHeaderCard'), {
    loading: () => <div className="h-24 w-full bg-gray-100 rounded" />
});

const ArticleContentCard = dynamic(() => import('@/components/article/ArticleContentCard'), {
    loading: () => <div className="h-96 w-full bg-gray-100 rounded" />
});

const ArticleButtons = dynamic(() => import('@/components/article/ArticleButtons'), {
    loading: () => <div />
});

const ArticleFormModal = dynamic(() => import('@/components/shared/ArticleFormModal'), {
    loading: () => <div />
});

interface ArticleCardProps {
    topicId: number;
    article: API.ArticleDto;
}

const ArticleCard = ({ topicId, article }: ArticleCardProps) => {
    // Modal state
    const [isVisible, setIsVisible] = useState(false);
    const [currentArticle, setCurrentArticle] = useState<API.ArticleDto>();
    const [isPromptVisible, setIsPromptVisible] = useState(false);
    // Open modal
    const openModal = (article: API.ArticleDto) => {
        setCurrentArticle(article);
        setIsVisible(true);
    };

    // Close modal
    const closeModal = () => {
        setIsVisible(false);
    };

    // Handle modal success
    const handleModalSuccess = () => {
        closeModal();
    };
    const handleRegenerateArticle = async () => {
        try {
            await apiAiArticlesRegenerateContent({
                articleId: article.articleId,
            });
            window.location.reload();
        } catch (error) {
            message.error({ content: 'Failed to regenerate article content', key: 'regenerate' });
            console.error('Error regenerating article:', error);
        }
    };

    return (
        <div>
            <Row gutter={[16, 16]} style={{ margin: '0px' }}>
                <Col xs={24} lg={24}>
                    <ArticleHeaderCard article={article} />
                </Col>
                <Col xs={24} lg={24}>
                    <ArticleContentCard article={article} />
                </Col>
            </Row>
            <ArticleButtons
                article={article}
                onEditModal={() => openModal(article)}
                onRegenerateArticle={() => setIsPromptVisible(true)}
            />
            {/* Only render the modal when it's visible */}
            {isVisible && (
                <ArticleFormModal
                    visible={isVisible}
                    currentArticle={currentArticle}
                    onCancel={closeModal}
                    onSuccess={handleModalSuccess}
                />
            )}
            <AiPromptDrawer
                visible={isPromptVisible}
                onClose={() => setIsPromptVisible(false)}
                onConfirm={handleRegenerateArticle}
                promptType="regenerateContent"
                requestData={{
                    articleId: article.articleId,
                    userPrompt: ''
                }}
            />
        </div>
    );
};

export default ArticleCard; 