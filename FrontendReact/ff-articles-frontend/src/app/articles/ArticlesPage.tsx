import Title from "antd/es/typography/Title";
import { apiArticleGetByPage } from "@/api/contents/api/article";
import ArticleTable from "@/components/ArticleTable";

const ArticlesPage = async () => {
    let articleList: API.ArticleDto[] = [];
    let total = 0;

    try {
        const [articleRes] = await Promise.all([
            apiArticleGetByPage({
                PageSize: 12,
                DisplaySubArticles: true,
            })
        ]);
        articleList = articleRes.data?.data ?? [];
        total = articleRes.data?.counts ?? 0;
    } catch (e: any) {
        console.error("Failed fetching data:", e.message);
    }

    return (
        <div id="articlesPage" className="max-width-content">
            <Title level={3}>Articles list</Title>
            <ArticleTable defaultArticleList={articleList} defaultTotal={total} />
        </div>
    );
}

export default ArticlesPage;
