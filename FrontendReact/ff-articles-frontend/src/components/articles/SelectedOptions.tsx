import { useEffect, useState } from 'react';
import { Tag, Button, Space } from 'antd';
import { useArticlesContext } from '@/states/ArticlesContext';

interface SelectedOptionsProps {
  selectedTopicIds?: number[];
  selectedTagIds?: number[];
  onRemoveTopic: (topicId: number) => void;
  onRemoveTag: (tagId: number) => void;
  onClearAll: () => void;
}

const SelectedOptions = ({
  selectedTopicIds = [],
  selectedTagIds = [],
  onRemoveTopic,
  onRemoveTag,
  onClearAll
}: SelectedOptionsProps) => {
  const { topics, tags } = useArticlesContext();
  const [topicNames, setTopicNames] = useState<Record<number, string>>({});
  const [tagNames, setTagNames] = useState<Record<number, string>>({});

  // Prepare topic and tag names from context data instead of API calls
  useEffect(() => {
    // Map topic IDs to names
    const newTopicNames: Record<number, string> = {};
    topics.forEach(topic => {
      if (topic.topicId && selectedTopicIds.includes(topic.topicId)) {
        newTopicNames[topic.topicId] = topic.title || 'Unknown Topic';
      }
    });
    setTopicNames(newTopicNames);

    // Map tag IDs to names
    const newTagNames: Record<number, string> = {};
    tags.forEach(tag => {
      if (tag.tagId && selectedTagIds.includes(tag.tagId)) {
        newTagNames[tag.tagId] = tag.tagName || 'Unknown Tag';
      }
    });
    setTagNames(newTagNames);
  }, [selectedTopicIds, selectedTagIds, topics, tags]);

  const noFiltersSelected = selectedTopicIds.length === 0 && selectedTagIds.length === 0;

  return (
    <div className="py-2">
      <div className="flex justify-between items-center mb-2">
        <div className="text-gray-600">Selected filters:</div>
        <Button 
          size="small" 
          onClick={onClearAll}
          disabled={noFiltersSelected}
        >
          Clear all
        </Button>
      </div>
      
      {!noFiltersSelected && (
        <Space size={[0, 8]} wrap>
          {selectedTopicIds.map(id => (
            <Tag 
              key={`topic-${id}`}
              closable
              color="blue"
              className="m-1"
              onClose={() => onRemoveTopic(id)}
            >
              Topic: {topicNames[id] || 'Loading...'}
            </Tag>
          ))}
          
          {selectedTagIds.map(id => (
            <Tag 
              key={`tag-${id}`}
              closable
              color="green"
              className="m-1"
              onClose={() => onRemoveTag(id)}
            >
              Tag: {tagNames[id] || 'Loading...'}
            </Tag>
          ))}
        </Space>
      )}
    </div>
  );
};

export default SelectedOptions; 