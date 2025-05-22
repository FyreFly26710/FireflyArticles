import { Tag, Button, Space } from 'antd';
import { useArticles } from '@/hooks/useArticles';

const SelectedOptions = () => {
  const { topics, tags, filters, handleRemoveTopicFilter, handleRemoveTagFilter, handleClearFilters } = useArticles();
  const topicIds = filters.topicIds || [];
  const tagIds = filters.tagIds || [];

  // Get topic and tag names directly from the store data
  const getTopicName = (topicId: number) => {
    const topic = topics.find(t => t.topicId === topicId);
    return topic?.title || 'Unknown Topic';
  };

  const getTagName = (tagId: number) => {
    const tag = tags.find(t => t.tagId === tagId);
    return tag?.tagName || 'Unknown Tag';
  };

  const noFiltersSelected = topicIds.length === 0 && tagIds.length === 0;

  return (
    <div className="py-2">
      <div className="flex justify-between items-center mb-2">
        <div className="text-gray-600">Selected filters:</div>
        <Button
          size="small"
          onClick={handleClearFilters}
          disabled={noFiltersSelected}
        >
          Clear all
        </Button>
      </div>

      {!noFiltersSelected && (
        <Space size={[0, 8]} wrap>
          {topicIds.map(id => (
            <Tag
              key={`topic-${id}`}
              closable
              color="blue"
              className="m-1"
              onClose={() => handleRemoveTopicFilter(id)}
            >
              Topic: {getTopicName(id)}
            </Tag>
          ))}

          {tagIds.map(id => (
            <Tag
              key={`tag-${id}`}
              closable
              color="green"
              className="m-1"
              onClose={() => handleRemoveTagFilter(id)}
            >
              Tag: {getTagName(id)}
            </Tag>
          ))}
        </Space>
      )}
    </div>
  );
};

export default SelectedOptions; 