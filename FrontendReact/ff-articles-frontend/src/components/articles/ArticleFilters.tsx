import { Card } from 'antd';
import TopicSelector from './TopicSelector';
import TagSelector from './TagSelector';
import SelectedOptions from './SelectedOptions';

interface ArticleFiltersProps {
  keyword?: string;
  topicIds?: number[];
  tagIds?: number[];
  onSearch: (keyword: string) => void;
  onClearSearch: () => void;
  onTopicChange: (topicIds: number[]) => void;
  onTagChange: (tagIds: number[]) => void;
  onClearFilters: () => void;
  onRemoveTopic: (topicId: number) => void;
  onRemoveTag: (tagId: number) => void;
}

const ArticleFilters = ({
  keyword,
  topicIds = [],
  tagIds = [],
  onSearch,
  onClearSearch,
  onTopicChange,
  onTagChange,
  onClearFilters,
  onRemoveTopic,
  onRemoveTag
}: ArticleFiltersProps) => {
  return (
    <Card 
      style={{ marginBottom: '1rem'}} 
      bodyStyle={{ paddingBottom: '12px' }}
      className="mb-4"
    >
      <TopicSelector
        onTopicSelect={onTopicChange}
        selectedTopicIds={topicIds}
        keyword={keyword}
        onSearch={onSearch}
        onClearSearch={onClearSearch}
      />
      
      <div className="mt-3 mb-3">
        <TagSelector
          onTagSelect={onTagChange}
          selectedTagIds={tagIds}
        />
      </div>
      
      <SelectedOptions
        selectedTopicIds={topicIds}
        selectedTagIds={tagIds}
        onRemoveTopic={onRemoveTopic}
        onRemoveTag={onRemoveTag}
        onClearAll={onClearFilters}
      />
    </Card>
  );
};

export default ArticleFilters; 