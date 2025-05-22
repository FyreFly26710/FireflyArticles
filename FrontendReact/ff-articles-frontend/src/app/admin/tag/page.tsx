"use client";
import { Table, Button, Modal, Form, Input, Select } from 'antd';
import type { SortOrder } from 'antd/es/table/interface';
import { useTagManagement } from '@/hooks/admin/useAdminTag';

// Component definition
const TagManagement = () => {
  const {
    loading,
    modalVisible,
    form,
    selectedGroupFilter,
    filteredTags,
    handleEdit,
    handleSubmit,
    handleModalCancel,
    setSelectedGroupFilter,
    PREDEFINED_GROUPS,
    FILTER_OPTIONS,
  } = useTagManagement();

  // Table column definitions
  const columns = [
    {
      title: 'Tag Name',
      dataIndex: 'tagName',
      key: 'tagName',
      sorter: (a: API.TagDto, b: API.TagDto) => (a.tagName || '').localeCompare(b.tagName || ''),
      ellipsis: true,
    },
    {
      title: 'Group',
      dataIndex: 'tagGroup',
      key: 'tagGroup',
      sorter: (a: API.TagDto, b: API.TagDto) => (a.tagGroup || '').localeCompare(b.tagGroup || ''),
      defaultSortOrder: 'ascend' as SortOrder,
      ellipsis: true,
    },
    {
      title: 'Color',
      dataIndex: 'tagColour',
      key: 'tagColour',
      width: 80,
      render: (color: string | undefined) => (
        color ? (
          <div style={{
            width: 20,
            height: 20,
            backgroundColor: color,
            borderRadius: '50%',
            border: '1px solid #d9d9d9',
            margin: 'auto', // Center the color circle
          }} />
        ) : null
      ),
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 100,
      render: (_: any, record: API.TagDto) => (
        <Button type="link" onClick={() => handleEdit(record)}>
          Edit
        </Button>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ marginBottom: '16px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Select
          style={{ width: 240 }}
          placeholder="Filter by group"
          allowClear
          options={FILTER_OPTIONS}
          value={selectedGroupFilter}
          onChange={setSelectedGroupFilter}
        />
      </div>

      <Table
        columns={columns}
        dataSource={filteredTags}
        rowKey="tagId"
        loading={loading}
        scroll={{ x: 'max-content' }}
      />

      <Modal
        title="Edit Tag"
        open={modalVisible}
        onOk={handleSubmit}
        onCancel={handleModalCancel}
        destroyOnClose
        confirmLoading={loading}
      >
        <Form
          form={form}
          layout="vertical"
          name="tagEditForm"
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
          >
            <Select
              placeholder="Select or type a group"
              options={PREDEFINED_GROUPS.map(group => ({
                label: group,
                value: group,
              }))}
            />
          </Form.Item>
          <Form.Item
            name="tagColour"
            label="Color"
          >
            <Input type="color" style={{ width: '100%' }} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default TagManagement;
