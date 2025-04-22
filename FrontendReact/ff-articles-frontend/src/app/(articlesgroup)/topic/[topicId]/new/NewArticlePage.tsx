"use client";
import ArticleCard from "@/components/article/ArticleCard";
import ArticleSider from "@/components/article/ArticleSider";
import { useTags, useTopic, useTopicList } from "@/app/(articlesgroup)/topic/hooks";
import { Flex, Spin } from "antd";
import { useRouter } from "next/navigation";
import { useEffect } from "react";

interface PageProps {
    params: { topicId: number };
    searchParams?: { redirectId?: number };
}

const NewArticlePage = ({ params, searchParams }: PageProps) => {
    const { topicId } = params;
    const redirectId = searchParams?.redirectId ? Number(searchParams.redirectId) : undefined;
    const router = useRouter();

    // Redirect if no redirectId is provided
    useEffect(() => {
        if (!redirectId) {
            router.push('/error');
        }
    }, [redirectId, router]);

    const { topic, loading: topicLoading, error: topicError } = useTopic(topicId);
    const { topicList, loading: topicListLoading } = useTopicList();
    const { tagList, loading: tagListLoading } = useTags();

    const isLoading = topicLoading || topicListLoading || tagListLoading;

    if (isLoading) {
        return (
            <div className="flex justify-center items-center h-full">
                <Spin size="large" />
            </div>
        );
    }

    if (topicError || !topic) {
        return <div>Failed fetching topic, please refresh page.</div>;
    }

    if (!redirectId) {
        return null; // Will be redirected by useEffect
    }

    const nextSortNumber = (topic.articles && topic.articles.length > 0)
        ? Math.max(...topic.articles.map(article => article.sortNumber ?? 1)) + 1
        : 1;

    const article = {
        articleId: 0,
        articleType: "Article",
        abstract: "",
        content: "",
        topicId: topicId,
        sortNumber: nextSortNumber,
    } as API.ArticleDto;

    return (
        <div id="articlePage" className="max-width-content">
            <Flex gap={12}>
                <ArticleSider topic={topic} parentArticleId={article.parentArticleId} articleId={article.articleId} />
                <ArticleCard
                    isNewArticle={true}
                    redirectArticleId={redirectId}
                    article={article}
                    topic={topic}
                    topicList={topicList || []}
                    tagList={tagList || []}
                />
            </Flex>
        </div>
    );
}

export default NewArticlePage;