import { useEffect, useState } from 'react';
import { Select } from 'antd';
import { useArticlesContext } from '@/states/ArticlesContext';

interface TagSelectorProps {
  onTagSelect: (tagIds: number[]) => void;
  selectedTagIds?: number[];
}

const TagSelector = ({ onTagSelect, selectedTagIds = [] }: TagSelectorProps) => {
  const { tags } = useArticlesContext();
  const [loading, setLoading] = useState(false);

  const handleChange = (value: number[]) => {
    onTagSelect(value);
  };

  return (
    <Select
      mode="multiple"
      placeholder={loading ? "Loading tags..." : "Select tags"}
      value={selectedTagIds}
      onChange={handleChange}
      style={{ width: '100%' }}
      loading={loading}
      disabled={loading}
      optionFilterProp="label"
      options={tags.map(tag => ({
        label: tag.tagName,
        value: tag.tagId,
      }))}
      showSearch
    />
  );
};

export default TagSelector; 