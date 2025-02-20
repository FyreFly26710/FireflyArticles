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
}

/**
 * Article table component
 * @constructor
 */
export default function ArticleTable(props: Props) {
  const { defaultArticleList, defaultTotal } = props;
  const actionRef = useRef<ActionType>();
  const [articleList, setArticleList] = useState<API.ArticleDto[]>(defaultArticleList || []);
  const [total, setTotal] = useState<number>(defaultTotal || 0);
  const [init, setInit] = useState<boolean>(true);

  /**
   * Table column configuration
   */
  const columns: ProColumns<API.ArticleDto>[] = [
    {
      title: "Article",
      dataIndex: "title",
      valueType: "text",
      hideInSearch: true,
      render(_, record) {
        return <Link href={`/article/${record.articleId}`}>{record.title}</Link>;
      },
    },
    {
      title: "Search",
      dataIndex: "searchText",
      valueType: "text",
      hideInTable: true,
    },
    {
      title: "Tags",
      dataIndex: "tagList",
      valueType: "select",
      fieldProps: {
        mode: "tags",
      },
      render: (_, record) => <TagList tagList={record.tags} />,
    },
  ];

  return (
    <div className="article-table">
      <ProTable<API.ArticleDto>
        actionRef={actionRef}
        size="large"
        search={{
          labelWidth: "auto",
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

          const sortField = Object.keys(sort)?.[0] || "createTime";
          const sortOrder = sort?.[sortField] || "descend";
          //@ts-ignore
          const { data, code } = await apiArticleGetByPage({
            ...params,
            sortField,
            sortOrder,
            ...filter,
          } as API.apiArticleGetByPageParams);

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
