import { Select, Space, Typography, Row, Col } from 'antd';
import { useArticles } from '@/hooks/useArticles';

const TagSelector = () => {
  const { tags, filters, handleTagChange } = useArticles();
  const tagIds = filters.tagIds || [];

  // Filter tags by specific categories
  const allowedCategories = ['Skill Level', 'Article Style', 'Tone'];
  const filteredTags = tags.filter(tag => allowedCategories.includes(tag.tagGroup || ''));

  // Group tags by tagGroup
  const groupedTags = filteredTags.reduce((acc, tag) => {
    const group = tag.tagGroup || 'Other';
    if (!acc[group]) {
      acc[group] = [];
    }
    acc[group].push(tag);
    return acc;
  }, {} as Record<string, typeof tags>);

  const handleChange = (value: number | null, group: string) => {
    // Get all tags from other groups
    const otherGroupTags = Object.entries(groupedTags)
      .filter(([g]) => g !== group)
      .flatMap(([_, tags]) => tags)
      .map(tag => tag.tagId)
      .filter(id => id !== undefined) as number[];

    // Get existing selections from other groups
    const otherGroupSelections = tagIds.filter(id => otherGroupTags.includes(id));

    // Combine with the new selection if it exists
    const newSelections = value !== null
      ? [...otherGroupSelections, value]
      : otherGroupSelections;

    handleTagChange(newSelections);
  };

  return (
    <Space direction="vertical" style={{ width: '100%' }}>
      <Row gutter={[16, 16]}>
        {Object.entries(groupedTags).map(([group, groupTags]) => (
          <Col key={group} span={4.8}>
            <Space direction="horizontal" style={{ width: '100%' }}>
              <Typography.Text strong>{group}</Typography.Text>
              <Select
                value={tagIds.find(id =>
                  groupTags.some(tag => tag.tagId === id)
                )}
                onChange={(value) => handleChange(value, group)}
                style={{ width: '100%', minWidth: '150px' }}
                optionFilterProp="label"
                placeholder="Select tag"
                options={groupTags.map(tag => ({
                  label: tag.tagName,
                  value: tag.tagId,
                }))}
                showSearch
                allowClear
              />
            </Space>
          </Col>
        ))}
      </Row>
    </Space>
  );
};

export default TagSelector; 