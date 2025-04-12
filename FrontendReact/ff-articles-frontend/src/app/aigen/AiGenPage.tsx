'use client';

import React, { useState } from 'react';
import { Card, Space, Spin, Button } from 'antd';
import Title from "antd/es/typography/Title";
import ArticleGenerationForm from './components/ArticleGenerationForm';
import { apiAiArticlesGenerateList } from '@/api/ai/api/aiarticles';
import styles from './aigen.module.css';
import ArticleResults from './components/ArticleResults';

const AiGenPage = () => {
    const [loading, setLoading] = useState(false);
    const [results, setResults] = useState<API.ArticlesAIResponseDto['data']>();
    const [showForm, setShowForm] = useState(true);

    const handleGenerateArticles = async (topic: string, articleCount: number) => {
        setLoading(true);
        setResults(undefined);
        try {
            const response = await apiAiArticlesGenerateList({
                topic,
                articleCount
            });
            setResults(response.data);
            setShowForm(false);
        } catch (error) {
            console.error('Error generating articles:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleUpdateResults = (updatedResults: API.ArticlesAIResponseDto['data']) => {
        setResults(updatedResults);
    };

    const handleNewGeneration = () => {
        setShowForm(true);
        // We keep the results in state until new generation completes
    };

    const handleClearResults = () => {
        setResults(undefined);
        setShowForm(true);
    };

    return (
        <div className={styles.container}>
            <Space direction="vertical" style={{ width: '100%' }} size="large">
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Title level={3}>AI Article Generator</Title>
                    {results && !showForm && (
                        <Space>
                            <Button onClick={handleNewGeneration}>New Generation</Button>
                            <Button danger onClick={handleClearResults}>Clear Results</Button>
                        </Space>
                    )}
                </div>

                {showForm && (
                    <Card title="Generate Articles" bordered={false} className={styles['article-results-card']}>
                        <ArticleGenerationForm onSubmit={handleGenerateArticles} loading={loading} />
                    </Card>
                )}

                {loading && (
                    <Card bordered={false} className={styles['article-results-card']}>
                        <div className={styles['loading-container']}>
                            <Spin size="large" />
                            <div className={styles['loading-text']}>
                                Generating creative article ideas based on your topic...
                            </div>
                        </div>
                    </Card>
                )}

                {results && !loading && (
                    <ArticleResults
                        results={results}
                        onUpdateResults={handleUpdateResults}
                    />
                )}
            </Space>
        </div>
    );
}

export default AiGenPage;
