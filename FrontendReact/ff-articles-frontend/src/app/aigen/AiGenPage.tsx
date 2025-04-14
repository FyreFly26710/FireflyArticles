'use client';

import React from 'react';
import { Card, Space, Spin, Button } from 'antd';
import Title from "antd/es/typography/Title";
import ArticleGenerationForm from './components/ArticleGenerationForm';
import styles from './aigen.module.css';
import ArticleResults from './components/ArticleResults';
import { useAiGen } from './context/AiGenContext';

const AiGenPage = () => {
    const { 
        loading, 
        results, 
        showForm,
        newGeneration,
        clearResults
    } = useAiGen();

    return (
        <div className={styles.container}>
            <Space direction="vertical" style={{ width: '100%' }} size="large">
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Title level={3}>AI Article Generator</Title>
                    {results && !showForm && (
                        <Space>
                            <Button onClick={newGeneration}>New Generation</Button>
                            <Button danger onClick={clearResults}>Clear Results</Button>
                        </Space>
                    )}
                </div>

                {showForm && (
                    <Card title="Generate Articles" bordered={false} className={styles['article-results-card']}>
                        <ArticleGenerationForm />
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
                    <ArticleResults />
                )}
            </Space>
        </div>
    );
}

export default AiGenPage;
