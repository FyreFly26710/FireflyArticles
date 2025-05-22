import { Card, Row, Col } from 'antd';
import TopicSelector from './TopicSelector';
import TagSelector from './TagSelector';
import SelectedOptions from './SelectedOptions';
import SearchBar from './SearchBar';
import { useArticles } from '@/hooks/useArticles';

const ArticleFilters = () => {
  const {
    filters,
    handleSearch,
    handleClearSearch,
  } = useArticles();

  return (
    <Card
      style={{ marginBottom: '1rem' }}
      bodyStyle={{ paddingBottom: '12px' }}
      className="mb-4"
    >
      <TopicSelector />
      <div className="mt-3">
        <Row gutter={16} align="middle">
          <Col span={18}>
            <TagSelector />
          </Col>
          <Col span={6} className="mt-2 md:mt-0">
            <SearchBar
              onSearch={handleSearch}
              onClear={handleClearSearch}
              initialValue={filters.keyword}
            />
          </Col>
        </Row>
      </div>
      <SelectedOptions />
    </Card>
  );
};

export default ArticleFilters; 