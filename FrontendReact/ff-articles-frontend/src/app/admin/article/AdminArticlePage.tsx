"use client";
import { PageContainer, ProTable } from "@ant-design/pro-components";
import { Button } from "antd";
import React, { useEffect, useState } from "react";
import ArticleFormModal from "@/components/article/ArticleFormModal";
import TagModal from "@/components/admin/TagManagementModal";
import { getArticleColumns } from "../utils/articleColumns";
import { useArticleAdmin } from "../utils/useArticleAdmin";

const AdminArticlePage: React.FC = () => {
  const {
    updateModalVisible,
    setUpdateModalVisible,
    actionRef,
    currentRow,
    setCurrentRow,
    tagList,
    fetchTagList,
    handleDelete,
    fetchArticles,
  } = useArticleAdmin();

  const [isTagModalVisible, setIsTagModalVisible] = useState(false);

  useEffect(() => {
    fetchTagList();
  }, []);

  const handleTagsChange = async () => {
    await fetchTagList();
  };

  return (
    <PageContainer>
      <ProTable<API.ArticleDto>
        headerTitle={"Query Table"}
        actionRef={actionRef}
        rowKey="key"
        search={{
          labelWidth: 120,
        }}
        toolBarRender={() => [
          <Button
            color="default"
            variant="outlined"
            key="primary"
            onClick={() => {
              setIsTagModalVisible(true);
            }}
          >
            Manage Tags
          </Button>,
        ]}
        request={fetchArticles}
        columns={getArticleColumns(setCurrentRow, setUpdateModalVisible, handleDelete)}
      />

      <ArticleFormModal
        parentArticleList={[]}
        tagList={tagList}
        modifyContent={false}
        currentArticle={currentRow as API.ArticleDto}
        visible={updateModalVisible}
        onSubmit={() => { setUpdateModalVisible(false); actionRef.current?.reload(); }}
        onCancel={() => { setUpdateModalVisible(false); }}
      />

      <TagModal
        visible={isTagModalVisible}
        tags={tagList}
        onCancel={() => setIsTagModalVisible(false)}
        onTagsChange={handleTagsChange}
      />
    </PageContainer>
  );
};

export default AdminArticlePage;
