import { apiTopicGetByPage } from "@/api/contents/api/topic";
import CategoryTopics from "@/components/topics/CategoryTopics";
import { getTopicsByCategory } from "@/libs/utils/articleUtils";

const TopicsPage = async () => {
    try {
        const response = await apiTopicGetByPage({
            PageNumber: 1,
            PageSize: 100,
        });

        const topicsData = response.data?.data || [];

        const topicsByCategory = getTopicsByCategory(topicsData);

        return (
            <div className="max-width-content flex-grow">
                <CategoryTopics
                    topicsByCategory={topicsByCategory}
                />
            </div>
        );
    } catch (error) {
        console.error("Failed to fetch topics:", error);
        return (
            <div className="error-container flex-grow flex items-center justify-center">
                <h2>Error loading topics</h2>
                <p>Something went wrong while fetching topics. Please try again later.</p>
            </div>
        );
    }
};

export default TopicsPage;