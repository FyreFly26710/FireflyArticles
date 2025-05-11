"use client";

import { ReactNode, useState } from 'react';
import { Layout } from 'antd';
import dynamic from 'next/dynamic';
import { useParams } from 'next/navigation';

// Dynamically import components to reduce initial bundle size
const ArticleSider = dynamic(() => import('@/components/article/ArticleSider'), {
    loading: () => <div className="h-full w-64 bg-gray-100" />
});

interface ArticleLayoutProps {
    children: ReactNode;
    topic: API.TopicDto | null;
}

export default function ArticleLayout({ children, topic }: ArticleLayoutProps) {
    const [siderCollapsed, setSiderCollapsed] = useState(false);
    const params = useParams();

    // Extract articleId from params
    let articleId: number | null = null;
    if (params.articleId && typeof params.articleId === 'string') {
        articleId = parseInt(params.articleId);
        if (isNaN(articleId)) articleId = null;
    }

    const toggleSider = () => {
        setSiderCollapsed(!siderCollapsed);
    };

    return (
        <Layout className="min-h-screen">
            {topic ? (
                <ArticleSider
                    topic={topic}
                    collapsed={siderCollapsed}
                    onCollapse={toggleSider}
                    articleId={articleId}
                />
            ) : (
                <div
                    style={{
                        width: '300px',
                        height: 'calc(100vh - 64px)',
                        position: 'fixed',
                        top: 64,
                        left: 0,
                        background: '#f0f0f0'
                    }}
                />
            )}
            <Layout
                className="site-layout"
                style={{
                    marginLeft: siderCollapsed ? '64px' : '300px',
                    transition: 'margin-left 0.2s'
                }}
            >
                {children}
            </Layout>
        </Layout>
    );
} 