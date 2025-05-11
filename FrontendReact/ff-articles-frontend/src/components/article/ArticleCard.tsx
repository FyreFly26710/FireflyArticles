"use client";

import { Row, Col } from 'antd';
import { useState } from 'react';
import dynamic from 'next/dynamic';

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
    const [currentArticle, setCurrentArticle] = useState<API.ArticleDto | null>(null);

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
        // You could add a callback to refresh article data here if needed
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
                topicId={topicId}
                onEditModal={() => openModal(article)}
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
        </div>
    );
};

export default ArticleCard; 