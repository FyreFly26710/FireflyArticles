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

  const handleChange = (value: number[], group: string) => {
    // Get all tags from other groups
    const otherGroupTags = Object.entries(groupedTags)
      .filter(([g]) => g !== group)
      .flatMap(([_, tags]) => tags)
      .map(tag => tag.tagId)
      .filter(id => id !== undefined) as number[];

    // Get all tag IDs for the current group
    const currentGroupTagIds = groupedTags[group]
      .map(tag => tag.tagId)
      .filter(id => id !== undefined) as number[];

    // Check if "All" is selected (it will be the first value if selected)
    const isAllSelected = value.includes(-1);

    // If "All" is selected, use all tags from the current group
    const newGroupSelections = isAllSelected ? currentGroupTagIds : value;

    // Combine new selections with existing selections from other groups
    const otherGroupSelections = tagIds.filter(id => otherGroupTags.includes(id));
    const newSelections = [...otherGroupSelections, ...newGroupSelections];

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
                mode="multiple"
                value={tagIds.filter(id =>
                  groupTags.some(tag => tag.tagId === id)
                )}
                onChange={(value) => handleChange(value, group)}
                style={{ width: '100%', minWidth: '150px' }}
                optionFilterProp="label"
                placeholder="Select tags"
                options={[
                  { label: 'All', value: -1 },
                  ...groupTags.map(tag => ({
                    label: tag.tagName,
                    value: tag.tagId,
                  }))
                ]}
                showSearch
                maxTagCount={3}
                maxTagTextLength={10}
                maxTagPlaceholder={(omittedValues) => `+${omittedValues.length} more`}
              />
            </Space>
          </Col>
        ))}
      </Row>
    </Space>
  );
};

export default TagSelector; 