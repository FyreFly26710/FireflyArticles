import { Card, Row, Col } from 'antd';
import TopicSelector from './TopicSelector';
import TagSelector from './TagSelector';
import SelectedOptions from './SelectedOptions';
import SearchBar from './SearchBar';

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
      style={{ marginBottom: '1rem' }}
      bodyStyle={{ paddingBottom: '12px' }}
      className="mb-4"
    >
        <TopicSelector
            onTopicSelect={onTopicChange}
            selectedTopicIds={topicIds}
        />
        <TagSelector
            onTagSelect={onTagChange}
            selectedTagIds={tagIds}
        />
      <div className="mt-3">
        <Row gutter={16} align="middle">
          <Col span={18}>
            <SelectedOptions
                selectedTopicIds={topicIds}
                selectedTagIds={tagIds}
                onRemoveTopic={onRemoveTopic}
                onRemoveTag={onRemoveTag}
                onClearAll={onClearFilters}
            />
          </Col>
          <Col span={6} className="mt-2 md:mt-0">
            <SearchBar
              onSearch={onSearch}
              onClear={onClearSearch}
              initialValue={keyword}
            />
          </Col>
        </Row>
      </div>


    </Card>
  );
};

export default ArticleFilters; 