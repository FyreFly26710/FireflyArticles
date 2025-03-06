"use server";
import Title from "antd/es/typography/Title";
import "./index.css";
import { apiArticleGetByPage } from "@/api/contents/api/article";
import ArticleTable from "@/components/ArticleTable";
import { apiTopicGetByPage } from "@/api/contents/api/topic";
import { apiTagGetAll } from "@/api/contents/api/tag";


export default async function ArticlesPage() {
    let articleList:any = [];
    let topicList:any = [];
    let tagList:any = [];

    let total = 0;

    try {
        const articleRes = await apiArticleGetByPage({
            PageSize: 12,
            IncludeContent:false,
            IncludeSubArticles:false,
            IncludeUser:false,
            DisplaySubArticles:true,
        });
        articleList = articleRes.data.data ?? [];
        //@ts-ignore
        total = articleRes.data.counts ?? 0;
    } catch (e:any) {
        console.error("Failed fetching articles, " + e.message);
    }
    try {
        const topicRes = await apiTopicGetByPage({
            PageSize: 100,
            IncludeContent:false,
            IncludeArticles:false,
            IncludeSubArticles:false,
            IncludeUser:false,
        });
        topicList = topicRes.data.data ?? [];
    } catch (e:any) {
        console.error("Failed topics, " + e.message);
    }
    try {
        const tagRes = await apiTagGetAll();
        tagList = tagRes.data ?? [];
    } catch (e:any) {
        console.error("Failed tags, " + e.message);
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
