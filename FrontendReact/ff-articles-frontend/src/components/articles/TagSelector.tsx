import { useEffect, useState } from 'react';
import { Select, Space, Typography, Row, Col } from 'antd';
import { useArticlesContext } from '@/states/ArticlesContext';

interface TagSelectorProps {
  onTagSelect: (tagIds: number[]) => void;
  selectedTagIds?: number[];
}

const TagSelector = ({ onTagSelect, selectedTagIds = [] }: TagSelectorProps) => {
  const { tags } = useArticlesContext();
  const [loading, setLoading] = useState(false);

  // Group tags by tagGroup
  const groupedTags = tags.reduce((acc, tag) => {
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
    const otherGroupSelections = selectedTagIds.filter(id => otherGroupTags.includes(id));
    const newSelections = [...otherGroupSelections, ...newGroupSelections];
    
    onTagSelect(newSelections);
  };

  // Convert grouped tags to array for easier mapping
  const groupEntries = Object.entries(groupedTags);

  return (
    <Space direction="vertical" style={{ width: '100%' }}>
      <Row gutter={[16, 16]}>
        {groupEntries.map(([group, groupTags]) => (
          <Col key={group} span={4.8}>
            <Space direction="vertical" style={{ width: '100%' }}>
              <Typography.Text strong>{group}</Typography.Text>
              <Select
                mode="multiple"
                value={selectedTagIds.filter(id => 
                  groupTags.some(tag => tag.tagId === id)
                )}
                onChange={(value) => handleChange(value, group)}
                style={{ width: '100%', minWidth: '200px' }}
                loading={loading}
                disabled={loading}
                optionFilterProp="label"
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