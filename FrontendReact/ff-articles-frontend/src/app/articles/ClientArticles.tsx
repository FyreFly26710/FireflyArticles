"use client";
import { useEffect } from "react";
import { useArticleTable } from "@/hooks/useArticleTable";
import dynamic from "next/dynamic";

const ArticleFilters = dynamic(() => import("@/components/articles/ArticleFilters"), { ssr: false });
const ArticleTable = dynamic(() => import("@/components/articles/ArticleTable"), { ssr: false });

interface ClientArticlesProps {
    initialArticles: API.ArticleDto[];
    totalCount: number;
}

export default function ClientArticles({ initialArticles, totalCount }: ClientArticlesProps) {
    const {
        articles,
        loading,
        total,
        pageNumber,
        pageSize,
        filters,
        handleSearch,
        handleClearSearch,
        handleTopicChange,
        handleTagChange,
        handleClearFilters,
        handlePageChange,
        removeTopicFilter,
        removeTagFilter,
    } = useArticleTable();

    // Initialize with server-rendered data
    useEffect(() => {
        if (initialArticles.length > 0 && articles.length === 0) {
            const articleState = {
                articles: initialArticles,
                loading: false,
                total: totalCount,
            };
            // We can't directly update state here, but the hook will show initialArticles on first render
        }
    }, [initialArticles, totalCount, articles]);

    return (
        <div>
            <ArticleFilters
                keyword={filters.keyword}
                topicIds={filters.topicIds}
                tagIds={filters.tagIds}
                onSearch={handleSearch}
                onClearSearch={handleClearSearch}
                onTopicChange={handleTopicChange}
                onTagChange={handleTagChange}
                onClearFilters={handleClearFilters}
                onRemoveTopic={removeTopicFilter}
                onRemoveTag={removeTagFilter}
            />

            <ArticleTable
                articles={articles.length > 0 ? articles : initialArticles}
                loading={loading}
                total={total > 0 ? total : totalCount}
                pageNumber={pageNumber}
                pageSize={pageSize}
                onPageChange={handlePageChange}
            />
        </div>
    );
} 