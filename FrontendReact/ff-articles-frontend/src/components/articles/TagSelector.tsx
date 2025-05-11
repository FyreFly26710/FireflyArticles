import { useEffect, useState } from 'react';
import { Select } from 'antd';
import { apiTagGetAll } from '@/api/contents/api/tag';

interface TagSelectorProps {
  onTagSelect: (tagIds: number[]) => void;
  selectedTagIds?: number[];
}
const TagSelector = ({ onTagSelect, selectedTagIds = [] }: TagSelectorProps) => {
  const [loading, setLoading] = useState(true);
  const [tags, setTags] = useState<API.TagDto[]>([]);

  useEffect(() => {
    const fetchTags = async () => {
      try {
        const response = await apiTagGetAll();
        if (response?.data) {
          setTags(response.data);
        }
      } catch (error) {
        console.error("Failed to fetch tags:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchTags();
  }, []);

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