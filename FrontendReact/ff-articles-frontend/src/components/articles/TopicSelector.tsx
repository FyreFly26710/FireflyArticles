import { useEffect, useState } from 'react';
import { Tabs, Spin, Empty, List } from 'antd';
import { useArticlesContext } from '@/states/ArticlesContext';

interface TopicSelectorProps {
  onTopicSelect: (topicIds: number[]) => void;
  selectedTopicIds?: number[];
}

const TopicSelector = ({
  onTopicSelect,
  selectedTopicIds = []
}: TopicSelectorProps) => {
  const { topicsByCategory } = useArticlesContext();
  const [selectedTopics, setSelectedTopics] = useState<Set<number>>(new Set(selectedTopicIds));
  const [loading, setLoading] = useState(false);

  // Update selectedTopics when selectedTopicIds changes
  useEffect(() => {
    setSelectedTopics(new Set(selectedTopicIds));
  }, [selectedTopicIds]);

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
      <div className="mb-2">
        <Tabs
          type="card"
          className="topic-selector-tabs"
          tabPosition="top"
          tabBarGutter={8}
          tabBarStyle={{
            overflow: 'auto',
            whiteSpace: 'nowrap',
            marginBottom: '8px'
          }}
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
                      className={`topic-item cursor-pointer p-2 rounded border ${selectedTopics.has(topic.topicId!) ? 'bg-blue-50 border-blue-500' : 'border-gray-200 hover:border-blue-300'
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
  );
};

export default TopicSelector; 