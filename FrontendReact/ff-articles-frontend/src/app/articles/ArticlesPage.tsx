import { Suspense } from "react";
import { apiTopicGetByPage } from "@/api/contents/api/topic";
import { getTopicsByCategory } from "@/libs/utils/articleUtils";
import { apiTagGetAll } from "@/api/contents/api/tag";
import ClientArticles from "./ClientArticles";

const ArticlesPage = async () => {
    try {
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
                    <ClientArticles
                        initialTopics={topics}
                        initialTags={tags}
                        initialTopicsByCategory={topicsByCategory}
                    />
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