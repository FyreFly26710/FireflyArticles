export const getTopicsByCategory = (topicsData: API.TopicDto[]) => {
    const topicsByCategory: Record<string, API.TopicDto[]> = {};

    topicsData.forEach(topic => {
        const category = topic.category || "Uncategorized";

        if (!topicsByCategory[category]) {
            topicsByCategory[category] = [];
        }

        topicsByCategory[category].push(topic);
    });

    return topicsByCategory;
};

