"use client";
import { useEffect, useCallback } from "react";
import dynamic from "next/dynamic";
import { useArticles } from "@/hooks/useArticles";

const ArticleFilters = dynamic(() => import("@/components/articles/ArticleFilters"), { ssr: false });
const ArticleTable = dynamic(() => import("@/components/articles/ArticleTable"), { ssr: false });

interface ClientArticlesProps {
    initialTopics: API.TopicDto[];
    initialTags: API.TagDto[];
    initialTopicsByCategory: Record<string, API.TopicDto[]>;
}

export default function ClientArticles({
    initialTopics,
    initialTags,
    initialTopicsByCategory
}: ClientArticlesProps) {
    const {
        articles,
        loading,
        total,
        pageNumber,
        pageSize,
        filters,
        initializeData,
        handlePageChange,
        fetchArticles
    } = useArticles();

    // Initialize data when component mounts
    useEffect(() => {
        initializeData({
            topics: initialTopics,
            tags: initialTags,
            topicsByCategory: initialTopicsByCategory
        });
    }, []);

    // Fetch articles when filters or pagination changes
    useEffect(() => {
        // Skip initial fetch if no filters are set yet
        if (!filters) return;

        fetchArticles();
    }, [filters?.keyword, filters?.topicIds, filters?.tagIds, pageNumber, pageSize]);

    return (
        <div>
            <ArticleFilters />
            <ArticleTable
                articles={articles}
                loading={loading}
                total={total}
                pageNumber={pageNumber}
                pageSize={pageSize}
                onPageChange={handlePageChange}
            />
        </div>
    );
} 