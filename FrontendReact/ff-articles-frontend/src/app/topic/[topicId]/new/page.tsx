"use server";
import ArticleCard from "@/app/topic/components/ArticleCard";
import ArticleSider from "@/app/topic/components/ArticleSider";
import { fetchTags, fetchTopic, fetchTopicList } from "@/app/topic/utils/fetcher";
import { Flex } from "antd";
import { redirect } from "next/navigation";

interface PageProps {
    params: { topicId: number };
    searchParams?: { redirectId?: number };
}

export default async function NewArticlePage({ params, searchParams }: PageProps) {
    const { topicId } = params;
    const redirectId = searchParams?.redirectId ? searchParams.redirectId : undefined;

    if (!redirectId) { redirect('/error'); }

    const topic = await fetchTopic(topicId);
    if (!topic) return <div>Failed fetching topics, please refresh page.</div>;

    const [topicList, tagList] = await Promise.all([
        fetchTopicList(),
        fetchTags(),
    ]);

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
