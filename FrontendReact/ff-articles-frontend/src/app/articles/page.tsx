"use server";
import Title from "antd/es/typography/Title";
import "./index.css";
import { apiArticleGetByPage } from "@/api/contents/api/article";
import ArticleTable from "@/components/ArticleTable";


// @ts-ignore
export default async function ArticlesPage({ searchParams }) {
    // get searchText from URL
    const { q: searchText } = searchParams;
    let articleLilst:any = [];
    let total = 0;

    try {
        const articleRes = await apiArticleGetByPage({
            PageSize: 12
        });
        articleLilst = articleRes.data.data ?? [];
        //@ts-ignore
        total = articleRes.data.counts ?? 0;
    } catch (e:any) {
        console.error("Failed fetching articles, " + e.message);
    }

    return (
        <div id="articlesPage" className="max-width-content">
            <Title level={3}>Articles list</Title>
            <ArticleTable
                defaultArticleList={articleLilst}
                defaultTotal={total}
            />
        </div>
    );
}
