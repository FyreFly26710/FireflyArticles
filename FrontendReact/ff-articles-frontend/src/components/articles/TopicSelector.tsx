import { useEffect, useState } from 'react';
import { Tabs, Spin, Empty, List, Row, Col } from 'antd';
import { apiTopicGetByPage } from '@/api/contents/api/topic';
import { getTopicsByCategory } from '@/libs/utils/articleUtils';
import SearchBar from './SearchBar';

interface TopicSelectorProps {
  onTopicSelect: (topicIds: number[]) => void;
  selectedTopicIds?: number[];
  keyword?: string;
  onSearch: (keyword: string) => void;
  onClearSearch: () => void;
}

const TopicSelector = ({ 
  onTopicSelect, 
  selectedTopicIds = [], 
  keyword = '',
  onSearch,
  onClearSearch
}: TopicSelectorProps) => {
  const [loading, setLoading] = useState(true);
  const [topicsByCategory, setTopicsByCategory] = useState<Record<string, API.TopicDto[]>>({});
  const [selectedTopics, setSelectedTopics] = useState<Set<number>>(new Set(selectedTopicIds));

  // Update selectedTopics when selectedTopicIds changes
  useEffect(() => {
    setSelectedTopics(new Set(selectedTopicIds));
  }, [selectedTopicIds]);

  useEffect(() => {
    const fetchTopics = async () => {
      try {
        const response = await apiTopicGetByPage({
          PageNumber: 1,
          PageSize: 100,
          OnlyCategoryTopic: true,
        });

        if (response.data?.data) {
          const topicsData = response.data.data || [];
          const categorizedTopics = getTopicsByCategory(topicsData);
          setTopicsByCategory(categorizedTopics);
        }
      } catch (error) {
        console.error("Failed to fetch topics:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchTopics();
  }, []);

  const handleTopicClick = (topicId: number) => {
    const updatedSelectedTopics = new Set(selectedTopics);
    
    if (updatedSelectedTopics.has(topicId)) {
      updatedSelectedTopics.delete(topicId);
    } else {
      updatedSelectedTopics.add(topicId);
    }
    
    setSelectedTopics(updatedSelectedTopics);
    onTopicSelect(Array.from(updatedSelectedTopics));
  };

  if (loading) {
    return <Spin size="large" className="flex justify-center py-4" />;
  }

  const categories = Object.keys(topicsByCategory).sort();
  if (categories.length === 0) {
    return <Empty description="No topics available" />;
  }

  return (
    <div>
      <div className="flex mb-2">
        <div className="flex-grow">
          <Tabs
            type="card"
            className="topic-selector-tabs"
            tabBarExtraContent={
              <div className="w-80">
                <SearchBar
                  onSearch={onSearch}
                  onClear={onClearSearch}
                  initialValue={keyword}
                />
              </div>
            }
            items={categories.map(category => ({
              key: category,
              label: category,
              children: (
                <List
                  grid={{ gutter: 8, xs: 1, sm: 2, md: 3, lg: 4 }}
                  dataSource={topicsByCategory[category]}
                  renderItem={(topic) => (
                    <List.Item>
                      <div
                        className={`topic-item cursor-pointer p-2 rounded border ${
                          selectedTopics.has(topic.topicId!) ? 'bg-blue-50 border-blue-500' : 'border-gray-200 hover:border-blue-300'
                        }`}
                        onClick={() => handleTopicClick(topic.topicId!)}
                      >
                        {topic.title}
                      </div>
                    </List.Item>
                  )}
                />
              )
            }))}
          />
        </div>
      </div>
    </div>
  );
};

export default TopicSelector; 