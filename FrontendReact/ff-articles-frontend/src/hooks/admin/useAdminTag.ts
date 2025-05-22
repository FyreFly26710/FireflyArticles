import { useEffect, useState, useCallback } from 'react';
import { Form, message } from 'antd';
import { apiTagGetAll, apiTagEditByRequest } from '@/api/contents/api/tag';

export interface TagFormData {
  tagId?: number;
  tagName: string;
  tagGroup?: string;
  tagColour?: string;
}

export const PREDEFINED_GROUPS = [
  "Skill Level",
  "Article Style",
  "Tone",
  "Focus Area",
  "Tech Stack/Language"
];

// Options for the filter dropdown
export const FILTER_OPTIONS = [
  ...PREDEFINED_GROUPS.map(group => ({ label: group, value: group })),
  { label: 'Other', value: 'other' } // Represents tags not in PREDEFINED_GROUPS
];

export const useTagManagement = () => {
  const [tags, setTags] = useState<API.TagDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [form] = Form.useForm<TagFormData>();
  const [editingTag, setEditingTag] = useState<TagFormData | null>(null);
  const [selectedGroupFilter, setSelectedGroupFilter] = useState<string | undefined>();

  // Function to fetch tags from the API
  const fetchTags = useCallback(async () => {
    setLoading(true);
    try {
      const response = await apiTagGetAll();
      if (response.data) {
        setTags(response.data);
      } else {
        setTags([]); // Ensure tags is an empty array if no data
        message.warning('No tags found or failed to parse response.');
      }
    } catch (error) {
      console.error('Failed to fetch tags:', error);
      message.error('Failed to fetch tags. See console for details.');
      setTags([]); // Reset tags on error
    } finally {
      setLoading(false);
    }
  }, []);

  // Fetch tags on initial mount
  useEffect(() => {
    fetchTags();
  }, [fetchTags]);

  // Function to handle opening the edit modal and setting form values
  const handleEdit = useCallback((record: API.TagDto) => {
    const tagData: TagFormData = {
      tagId: record.tagId,
      tagName: record.tagName || '',
      tagGroup: record.tagGroup,
      tagColour: record.tagColour,
    };
    setEditingTag(tagData);
    form.setFieldsValue(tagData); // Set values for the antd form
    setModalVisible(true);
  }, [form]);

  // Function to handle form submission (editing a tag)
  const handleSubmit = useCallback(async () => {
    try {
      const values = await form.validateFields();
      if (editingTag?.tagId) {
        await apiTagEditByRequest({
          tagId: editingTag.tagId,
          tagName: values.tagName,
          tagGroup: values.tagGroup,
          tagColour: values.tagColour,
        });
        message.success('Tag updated successfully');
        setModalVisible(false);
        setEditingTag(null); // Clear editing tag
        fetchTags(); // Refresh the tags list
      } else {
        // Logic for creating a new tag could be added here if needed
        message.info('No tag selected for editing or create functionality not implemented.');
      }
    } catch (errorInfo) {
      console.error('Validation Failed or API error:', errorInfo);
      message.error('Operation failed. Please check form values or console for details.');
    }
  }, [form, editingTag, fetchTags]);

  // Function to get filtered tags based on the selected group
  const getFilteredTags = useCallback((): API.TagDto[] => {
    if (!selectedGroupFilter) return tags;
    if (selectedGroupFilter === 'other') {
      return tags.filter(tag => !tag.tagGroup || !PREDEFINED_GROUPS.includes(tag.tagGroup));
    }
    return tags.filter(tag => tag.tagGroup === selectedGroupFilter);
  }, [tags, selectedGroupFilter]);

  const filteredTags = getFilteredTags();

  // Function to handle closing the modal
  const handleModalCancel = useCallback(() => {
    setModalVisible(false);
    setEditingTag(null);
    form.resetFields();
  }, [form]);

  return {
    tags,
    loading,
    modalVisible,
    form,
    editingTag,
    selectedGroupFilter,
    filteredTags,
    fetchTags,
    handleEdit,
    handleSubmit,
    handleModalCancel,
    setModalVisible,
    setSelectedGroupFilter,
    PREDEFINED_GROUPS,
    FILTER_OPTIONS,
  };
};
