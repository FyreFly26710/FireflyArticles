'use client';

import React from 'react';
import { Typography, Row, Col, Collapse, Space, Button, Alert, Tooltip } from 'antd';
import { ThunderboltOutlined } from '@ant-design/icons';
import ArticleGenCard from './ArticleGenCard';
import { useAiGenContext } from '@/states/AiGenContext';
import { useAiGenEdit } from '@/hooks/useAiGenEditArticle';

const { Title, Paragraph, Text } = Typography;
const { Panel } = Collapse;

interface ArticleGenCardListProps {
    parsedArticles: API.ArticlesAIResponse;
}

const ArticleGenCardList: React.FC<ArticleGenCardListProps> = ({ parsedArticles }) => {
    const { generationState, articleListRequest } = useAiGenContext();
    const { generateAllContent } = useAiGenEdit();

    if (!parsedArticles) return null;

    // Calculate generation status
    const totalArticles = parsedArticles.articles.length;
    const generatedArticles = Object.values(generationState).filter(state => typeof state.articleId === 'number').length;
    const loadingArticles = Object.values(generationState).filter(state => state.loading).length;

    const isAllGenerated = generatedArticles === totalArticles;
    const isSomeGenerated = generatedArticles > 0;
    const isSomeLoading = loadingArticles > 0;

    // Count pending articles (not generated and not loading)
    const pendingArticles = parsedArticles.articles.filter(article => {
        const state = generationState[article.sortNumber];
        return !state || (state.loading === false && typeof state.articleId !== 'number');
    }).length;

    const handleGenerateAll = () => {
        generateAllContent();
    };

    return (
        <Space direction="vertical" style={{ width: '100%' }} size="large">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <Title level={4}>Topic Info</Title>
                {pendingArticles > 0 && (
                    <Tooltip title={`Generate ${pendingArticles} pending articles`}>
                        <Button
                            type="primary"
                            icon={<ThunderboltOutlined />}
                            onClick={handleGenerateAll}
                            loading={isSomeLoading}
                        >
                            Generate Pending ({pendingArticles})
                        </Button>
                    </Tooltip>
                )}
            </div>

            <div>
                <Text strong>Category: </Text>
                <Text>{parsedArticles.category}</Text>
                <br />
                <Text strong>Topic ID: </Text>
                <Text>{parsedArticles.topicId}</Text>
            </div>

            {isSomeGenerated && !isAllGenerated && (
                <Alert
                    message={`Generated ${generatedArticles} of ${totalArticles} articles`}
                    type="info"
                    showIcon
                />
            )}

            {isAllGenerated && (
                <Alert
                    message="All articles have been generated successfully!"
                    type="success"
                    showIcon
                />
            )}

            {parsedArticles.aiMessage && (
                <Collapse ghost>
                    <Panel header="AI Message" key="1">
                        <Paragraph>{parsedArticles.aiMessage}</Paragraph>
                    </Panel>
                </Collapse>
            )}

            <Title level={4}>Generated Articles</Title>
            <Row gutter={[16, 16]}>
                {parsedArticles.articles.map((article) => (
                    <Col xs={24} sm={24} md={12} key={article.sortNumber}>
                        <ArticleGenCard article={article} />
                    </Col>
                ))}
            </Row>
        </Space>
    );
};

export default ArticleGenCardList; 