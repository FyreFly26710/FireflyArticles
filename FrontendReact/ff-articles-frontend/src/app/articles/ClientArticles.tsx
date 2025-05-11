"use client";
import { useEffect } from "react";
import dynamic from "next/dynamic";
import { useArticlesContext } from "@/states/ArticlesContext";

const ArticleFilters = dynamic(() => import("@/components/articles/ArticleFilters"), { ssr: false });
const ArticleTable = dynamic(() => import("@/components/articles/ArticleTable"), { ssr: false });

export default function ClientArticles() {
    const {
        articles,
        initialArticles,
        loading,
        total,
        initialTotal,
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
    } = useArticlesContext();

    // Use the initial articles if we haven't fetched any articles yet
    const displayArticles = articles.length > 0 || loading ? articles : initialArticles;
    // Use initial total count if we haven't fetched any results yet
    const displayTotal = total > 0 || loading ? total : initialTotal;

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
                articles={displayArticles}
                loading={loading}
                total={displayTotal}
                pageNumber={pageNumber}
                pageSize={pageSize}
                onPageChange={handlePageChange}
            />
        </div>
    );
} 