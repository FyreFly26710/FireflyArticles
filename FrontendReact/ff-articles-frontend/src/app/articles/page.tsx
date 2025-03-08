"use server";

import Title from "antd/es/typography/Title";
import "./index.css";
import { apiArticleGetByPage } from "@/api/contents/api/article";
import ArticleTable from "@/components/ArticleTable";
import { apiTopicGetByPage } from "@/api/contents/api/topic";
import { apiTagGetAll } from "@/api/contents/api/tag";

export default async function ArticlesPage() {
    let articleList: API.ArticleDto[] = [];
    let topicList: API.TopicDto[] = [];
    let tagList: API.TagDto[] = [];
    let total = 0;

    try {
        const [articleRes, topicRes, tagRes] = await Promise.all([
            apiArticleGetByPage({
                PageSize: 12,
                IncludeContent: false,
                IncludeSubArticles: false,
                IncludeUser: false,
                DisplaySubArticles: true,
            }),
            apiTopicGetByPage({
                PageSize: 100,
                IncludeContent: false,
                IncludeArticles: false,
                IncludeSubArticles: false,
                IncludeUser: false,
            }),
            apiTagGetAll()
        ]);
        //@ts-ignore
        articleList = articleRes.data?.data ?? [];
        //@ts-ignore
        total = articleRes.data?.counts ?? 0;
        //@ts-ignore
        topicList = topicRes.data?.data ?? [];
        //@ts-ignore
        tagList = tagRes.data ?? [];
    } catch (e: any) {
        console.error("Failed fetching data:", e.message);
    }

    return (
        <div id="articlesPage" className="max-width-content">
            <Title level={3}>Articles list</Title>
            <ArticleTable
                defaultArticleList={articleList}
                defaultTotal={total}
                topics={topicList}
                tags={tagList}
            />
        </div>
    );
}
