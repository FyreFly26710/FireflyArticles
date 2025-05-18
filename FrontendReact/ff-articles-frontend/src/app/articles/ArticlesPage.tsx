import { Suspense } from "react";
import { apiArticleGetByPage } from "@/api/contents/api/article";
import ClientArticles from "./ClientArticles";
import { apiTopicGetByPage } from "@/api/contents/api/topic";
import { getTopicsByCategory } from "@/libs/utils/articleUtils";
import { apiTagGetAll } from "@/api/contents/api/tag";
import { ArticlesProvider } from "@/states/ArticlesContext";


const ArticlesPage = async () => {
    try {
        // Fetch initial articles with server-side rendering
        // const articlesResponse = await apiArticleGetByPage({
        //     PageNumber: 1,
        //     PageSize: 10,
        //     IncludeUser: false,
        //     DisplaySubArticles: true,
        //     SortByRelevance: true,
        // });

        // const articles = articlesResponse.data?.data || [];
        // const totalCount = articlesResponse.data?.counts || 0;

        const topicsResponse = await apiTopicGetByPage({
            PageNumber: 1,
            PageSize: 100,
            OnlyCategoryTopic: true,
        });

        const topics = topicsResponse.data?.data || [];
        const topicsByCategory = getTopicsByCategory(topics);

        const tagsResponse = await apiTagGetAll();
        const tags = tagsResponse.data || [];

        return (
            <div className="max-width-content">
                <Suspense fallback={<div>Loading filters...</div>}>
                    <ArticlesProvider
                        initialTopics={topics}
                        initialTags={tags}
                        initialTopicsByCategory={topicsByCategory}
                    >
                        <ClientArticles />
                    </ArticlesProvider>
                </Suspense>
            </div>
        );
    } catch (error) {
        console.error("Failed to fetch articles:", error);
        return (
            <div className="error-container">
                <h2>Error loading articles</h2>
                <p>Something went wrong while fetching articles. Please try again later.</p>
            </div>
        );
    }
};

export default ArticlesPage;