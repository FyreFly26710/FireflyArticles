import { useEffect, useState } from 'react';
import { Tag, Button, Space } from 'antd';
import { apiTopicGetById } from '@/api/contents/api/topic';
import { apiTagGetById } from '@/api/contents/api/tag';

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
  const [topicNames, setTopicNames] = useState<Record<number, string>>({});
  const [tagNames, setTagNames] = useState<Record<number, string>>({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const fetchNames = async () => {
      if (selectedTopicIds.length === 0 && selectedTagIds.length === 0) return;
      
      setLoading(true);
      
      try {
        // Fetch topics
        const topicPromises = selectedTopicIds.map(async (id) => {
          try {
            const response = await apiTopicGetById({ id });
            if (response?.data?.title) {
              return { id, name: response.data.title };
            }
            return null;
          } catch (error) {
            console.error(`Failed to fetch topic ${id}:`, error);
            return null;
          }
        });

        // Fetch tags
        const tagPromises = selectedTagIds.map(async (id) => {
          try {
            const response = await apiTagGetById({ id });
            if (response?.data?.tagName) {
              return { id, name: response.data.tagName };
            }
            return null;
          } catch (error) {
            console.error(`Failed to fetch tag ${id}:`, error);
            return null;
          }
        });

        const topicResults = await Promise.all(topicPromises);
        const tagResults = await Promise.all(tagPromises);

        const newTopicNames: Record<number, string> = {};
        const newTagNames: Record<number, string> = {};

        topicResults.forEach(result => {
          if (result) {
            newTopicNames[result.id] = result.name;
          }
        });

        tagResults.forEach(result => {
          if (result) {
            newTagNames[result.id] = result.name;
          }
        });

        setTopicNames(newTopicNames);
        setTagNames(newTagNames);
      } catch (error) {
        console.error("Failed to fetch filter names:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchNames();
  }, [selectedTopicIds, selectedTagIds]);

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
              Topic: {topicNames[id]}
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
              Tag: {tagNames[id]}
            </Tag>
          ))}
        </Space>
      )}
    </div>
  );
};

export default SelectedOptions; 