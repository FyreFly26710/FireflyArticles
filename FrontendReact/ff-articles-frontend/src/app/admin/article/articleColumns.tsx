import type { ProColumns } from "@ant-design/pro-components";
import { Space, Typography } from "antd";
import TagList from "@/components/shared/TagList";
import MdEditor from "@/components/shared/MdEditor";

export const getArticleColumns = (
  setCurrentRow: (row: API.ArticleDto) => void,
  setUpdateModalVisible: (visible: boolean) => void,
  handleDelete: (row: API.ArticleDto) => Promise<boolean>
): ProColumns<API.ArticleDto>[] => [
    {
      title: "ID",
      dataIndex: "articleId",
      valueType: "text",
      hideInForm: true,
    },
    {
      title: "Title",
      dataIndex: "title",
      valueType: "text",
    },
    {
      title: "Description",
      dataIndex: "abstract",
      valueType: "text",
      hideInForm: true,
    },
    {
      title: "Content",
      dataIndex: "content",
      valueType: "text",
      hideInTable: true,
      hideInForm: true,
      hideInSearch: true,
      renderFormItem: (item, {
        //@ts-ignore
        fieldProps
      }, form) => {
        return <MdEditor {...fieldProps} />;
      },
    },
    {
      title: "Topic",
      dataIndex: "topicId",
      valueType: "text",
      render: (_, record) => {
        const tagList = record.topicTitle ? [record.topicTitle] : [];
        return <TagList tags={tagList} />;
      },
      sorter: true,
    },
    {
      title: "Tags",
      dataIndex: "tags",
      valueType: "text",
      fieldProps: {
        mode: "tags",
      },
      render: (_, record) => {
        const tagList = record.tags ? record.tags : [];
        return <TagList tags={tagList} />;
      },
    },
    {
      title: "Sort Num",
      dataIndex: "sortNumber",
      valueType: "text",
      sorter: true,
    },
    {
      title: "Hidden",
      dataIndex: "isHidden",
      valueType: "text",
      hideInSearch: true,
    },
    {
      title: "Created Time",
      sorter: true,
      dataIndex: "createTime",
      valueType: "date",
      hideInSearch: true,
      hideInForm: true,
    },
    {
      title: "Updated Time",
      sorter: true,
      dataIndex: "updateTime",
      valueType: "date",
      hideInSearch: true,
      hideInForm: true,
    },
    {
      title: "Actions",
      dataIndex: "option",
      valueType: "option",
      width: 130,
      render: (_, record) => (
        <Space size="middle">
          <Typography.Link
            onClick={() => {
              setCurrentRow(record);
              setUpdateModalVisible(true);
            }}
          >
            Modify
          </Typography.Link>
          <Typography.Link type="danger" onClick={() => handleDelete(record)}>
            Delete
          </Typography.Link>
        </Space>
      ),
    },
  ]; 