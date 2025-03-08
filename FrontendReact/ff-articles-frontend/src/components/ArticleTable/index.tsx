"use client";

import { useRef, useState } from "react";
import { ActionType, ProColumns, ProTable } from "@ant-design/pro-components";
import Link from "next/link";
import { TablePaginationConfig } from "antd";
import TagList from "../TagList";
import { apiArticleGetByPage } from "@/api/contents/api/article";

interface Props {
  defaultArticleList?: API.ArticleDto[];
  defaultTotal?: number;
  topics: API.TopicDto[];
  tags: API.TagDto[];
}

/**
 * Article table component
 * @constructor
 */
export default function ArticleTable(props: Props) {
  const { defaultArticleList, defaultTotal, topics, tags } = props;
  const actionRef = useRef<ActionType>();
  const [articleList, setArticleList] = useState<API.ArticleDto[]>(defaultArticleList || []);
  const [total, setTotal] = useState<number>(defaultTotal || 0);
  const [init, setInit] = useState<boolean>(true);

  /**
   * Table column configuration
   */
  const columns: ProColumns<API.ArticleDto>[] = [
    {
      title: "Search",
      dataIndex: "keyword",
      valueType: "text",
      hideInTable: true,
    },
    {
      title: "Article",
      dataIndex: "title",
      width: 100,
      ellipsis: true,
      valueType: "text",
      hideInSearch: true,
      render(_, record) {
        return <Link href={`/topic/${record.topicId}/article/${record.articleId}`}>{record.title}</Link>;
      },
    },
    {
      title: "Description",
      dataIndex: "abstraction",
      width: 120,
      ellipsis: true,
      hideInSearch: true
    },
    {
      title: "Topic",
      dataIndex: "topicTitle",
      width: 60,
      ellipsis: true,
      valueType: "select",
      fieldProps: {
        mode: "multiple",
        options: topics.map(t => ({
          label: t.title,
          value: t.topicId,
        })),
        showSearch: true,
      },
      render: (_, record) => record.topicTitle,

    },
    {
      title: "Tags",
      width: 120,
      ellipsis: true,
      dataIndex: "tagList",
      valueType: "select",
      fieldProps: {
        mode: "multiple",
        options: tags.map(t => ({
          label: t.tagName,
          value: t.tagId,
        })),
        showSearch: true,
      },
      render: (_, record) => <TagList tagList={record.tags} />,
    }
  ];

  return (
    <div className="article-table">
      <ProTable<API.ArticleDto>
        actionRef={actionRef}
        size="large"
        search={{
          labelWidth: "auto",
          collapsed: false,
          collapseRender: false,
        }}
        form={{
          initialValues: {},
        }}
        dataSource={articleList}
        pagination={
          {
            pageSize: 12,
            showTotal: (total) => `Total ${total} articles`,
            showSizeChanger: false,
            total,
          } as TablePaginationConfig
        }
        // @ts-ignore
        request={async (params, sort, filter) => {

          if (init) {
            setInit(false);
            // if default list is available, do not query
            if (defaultArticleList && defaultTotal) {
              return { success: true, data: defaultArticleList, total: defaultTotal };
            }
          }
          const sortField = Object.keys(sort)?.[0] || "SortNumber";
          const sortOrder = sort?.[sortField] || "ascend";
          const selectedTopicIds = Array.isArray(params.topicTitle)
            ? params.topicTitle
            : [params.topicTitle];

          const selectedTagIds = Array.isArray(params.tagList)
            ? params.tagList
            : [params.tagList];

          const response = await apiArticleGetByPage({
            PageNumber: params.current,
            PageSize: params.pageSize,
            SortField: sortField,
            SortOrder: sortOrder,
            Keyword: params.keyword,
            TopicIds: selectedTopicIds.filter(Boolean),
            TagIds: selectedTagIds.filter(Boolean),
            IncludeContent: false,
            IncludeSubArticles: false,
            IncludeUser: false,
            DisplaySubArticles: true,
          } as API.apiArticleGetByPageParams);
          //@ts-ignore
          const { data, code } = response;
          const newData = data?.data || [];
          //@ts-ignore
          const newTotal = data?.counts || 0;
          //@ts-ignore
          setArticleList(newData);
          setTotal(newTotal);

          return {
            success: code === 0,
            data: newData,
            total: newTotal,
          };
        }}
        columns={columns}
      />
    </div>
  );
}
