"use client";
import dynamic from "next/dynamic";
import { useArticlesContext } from "@/states/ArticlesContext";

const ArticleFilters = dynamic(() => import("@/components/articles/ArticleFilters"), { ssr: false });
const ArticleTable = dynamic(() => import("@/components/articles/ArticleTable"), { ssr: false });

export default function ClientArticles() {
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
    } = useArticlesContext();


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