"use server";
import { apiTopicGetByPage } from "@/api/contents/api/topic";
import CategoryTopics from "@/components/topics/CategoryTopics";

const TopicsPage = async () => {
    try {
        // Fetch topics with server-side rendering
        const response = await apiTopicGetByPage({
            PageNumber: 1,
            PageSize: 100, // Adjust as needed
            IncludeUser: true,
            OnlyCategoryTopic: false, // Get all topics including categories
            SortField: "category", // Sort by category to group them
            SortOrder: "asc"
        });

        const topicsData = response.data?.data || [];
        const totalCount = response.data?.counts || 0;

        // Process topics by category
        const topicsByCategory: Record<string, API.TopicDto[]> = {};
        
        topicsData.forEach(topic => {
            const category = topic.category || "Uncategorized";
            
            if (!topicsByCategory[category]) {
                topicsByCategory[category] = [];
            }
            
            topicsByCategory[category].push(topic);
        });

        return (
            <CategoryTopics 
                topicsByCategory={topicsByCategory}
            />
        );
    } catch (error) {
        console.error("Failed to fetch topics:", error);
        return (
            <div className="error-container">
                <h2>Error loading topics</h2>
                <p>Something went wrong while fetching topics. Please try again later.</p>
            </div>
        );
    }
};

export default TopicsPage;