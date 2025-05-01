"use client";

import { Row, Col } from 'antd';
import dynamic from 'next/dynamic';
import { useArticleModal } from '@/hooks/useArticleModal';

// Dynamically import components to reduce initial bundle size
const ArticleHeaderCard = dynamic(() => import('@/components/topic/ArticleHeaderCard'), {
    loading: () => <div className="h-24 w-full bg-gray-100 rounded" />
});

const ArticleContentCard = dynamic(() => import('@/components/topic/ArticleContentCard'), {
    loading: () => <div className="h-96 w-full bg-gray-100 rounded" />
});

const ArticleButtons = dynamic(() => import('@/components/topic/ArticleButtons'), {
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
    const { openModal } = useArticleModal();

    // Handle opening the edit modal with the current article
    const handleOpenModal = () => {
        openModal(article);
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
                articleId={article?.articleId ?? topicId}
                onEditModal={handleOpenModal}
            />
            <ArticleFormModal />
        </div>
    );
};

export default ArticleCard; 