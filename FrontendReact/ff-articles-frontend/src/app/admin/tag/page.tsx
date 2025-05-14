"use client";
import { Table, Button, Space, Modal, Form, Input, message, Select } from 'antd';
import type { SortOrder } from 'antd/es/table/interface';
import { useEffect, useState } from 'react';
import { apiTagGetAll, apiTagEditByRequest } from '@/api/contents/api/tag';

interface TagFormData {
  tagId?: number;
  tagName: string;
  tagGroup?: string;
  tagColour?: string;
}

const PREDEFINED_GROUPS = [
  "Skill Level",
  "Article Style",
  "Tone",
  "Focus Area",
  "Tech Stack/Language"
];

const FILTER_OPTIONS = [
  ...PREDEFINED_GROUPS.map(group => ({ label: group, value: group })),
  { label: 'Other', value: 'other' }
];

const TagManagement = () => {
  const [tags, setTags] = useState<API.TagDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [form] = Form.useForm();
  const [editingTag, setEditingTag] = useState<TagFormData | null>(null);
  const [selectedGroup, setSelectedGroup] = useState<string | undefined>();

  const fetchTags = async () => {
    setLoading(true);
    try {
      const response = await apiTagGetAll();
      if (response.data) {
        setTags(response.data);
      }
    } catch (error) {
      message.error('Failed to fetch tags');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTags();
  }, []);

  const handleEdit = (record: API.TagDto) => {
    setEditingTag({
      tagId: record.tagId,
      tagName: record.tagName || '',
      tagGroup: record.tagGroup,
      tagColour: record.tagColour,
    });
    form.setFieldsValue({
      tagName: record.tagName,
      tagGroup: record.tagGroup,
      tagColour: record.tagColour,
    });
    setModalVisible(true);
  };

  const handleSubmit = async () => {
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
        fetchTags();
      }
    } catch (error) {
      message.error('Operation failed');
    }
  };

  const getFilteredTags = () => {
    if (!selectedGroup) return tags;
    
    if (selectedGroup === 'other') {
      return tags.filter(tag => !PREDEFINED_GROUPS.includes(tag.tagGroup || ''));
    }
    
    return tags.filter(tag => tag.tagGroup === selectedGroup);
  };

  const columns = [
    {
      title: 'Tag Name',
      dataIndex: 'tagName',
      key: 'tagName',
      sorter: (a: API.TagDto, b: API.TagDto) => (a.tagName || '').localeCompare(b.tagName || ''),
    },
    {
      title: 'Group',
      dataIndex: 'tagGroup',
      key: 'tagGroup',
      sorter: (a: API.TagDto, b: API.TagDto) => (a.tagGroup || '').localeCompare(b.tagGroup || ''),
      defaultSortOrder: 'ascend' as SortOrder,
    },
    {
      title: 'Color',
      dataIndex: 'tagColour',
      key: 'tagColour',
      render: (color: string) => (
        color ? (
          <div style={{ 
            width: 20, 
            height: 20, 
            backgroundColor: color,
            borderRadius: '50%',
            border: '1px solid #d9d9d9'
          }} />
        ) : null
      ),
    },
    {
      title: 'Actions',
      key: 'actions',
      render: (_: any, record: API.TagDto) => (
        <Button type="link" onClick={() => handleEdit(record)}>
          Edit
        </Button>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ marginBottom: '16px' }}>
        <Select
          style={{ width: 200 }}
          placeholder="Filter by group"
          allowClear
          options={FILTER_OPTIONS}
          value={selectedGroup}
          onChange={setSelectedGroup}
        />
      </div>

      <Table
        columns={columns}
        dataSource={getFilteredTags()}
        rowKey="tagId"
        loading={loading}
      />

      <Modal
        title="Edit Tag"
        open={modalVisible}
        onOk={handleSubmit}
        onCancel={() => setModalVisible(false)}
        destroyOnClose
      >
        <Form
          form={form}
          layout="vertical"
        >
          <Form.Item
            name="tagName"
            label="Tag Name"
            rules={[{ required: true, message: 'Please input tag name!' }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="tagGroup"
            label="Group"
            rules={[{ required: true, message: 'Please select a group!' }]}
          >
            <Select
              placeholder="Select a group"
              options={PREDEFINED_GROUPS.map(group => ({
                label: group,
                value: group
              }))}
            />
          </Form.Item>
          <Form.Item
            name="tagColour"
            label="Color"
          >
            <Input type="color" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default TagManagement;
