"use client";

import { Row, Col } from 'antd';
import dynamic from 'next/dynamic';

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


interface ArticleCardProps {
    topicId: number;
    article: API.ArticleDto | null;
    error: string | null;
}

const ArticleCard = ({ topicId, article, error }: ArticleCardProps) => {
    if (error) {
        return (
            <div className="flex justify-center items-center h-screen">
                <p className="text-red-500">{error}</p>
            </div>
        );
    }

    if (!article) {
        return (
            <div className="flex justify-center items-center h-screen">
                <p>Article content not found or failed to load.</p>
            </div>
        );
    }

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
            <ArticleButtons topicId={topicId} />
        </div>
    );
};

export default ArticleCard; 