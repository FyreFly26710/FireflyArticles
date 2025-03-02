"use client";

import { useEffect, useState } from "react";
import { Button, Card, Dropdown, Flex, Input, MenuProps, message } from "antd";
import Title from "antd/es/typography/Title";
import MdViewer from "../MdViewer";
import MdEditor from "../MdEditor";
import TagList from "../TagList";
import "./index.css";
import ArticleFormModal from "../ArticleFormModal";
import { apiArticleEditByRequest } from "@/api/contents/api/article";
import { useRouter } from "next/navigation";
import { log } from "console";


const { TextArea } = Input;

interface Props {
    isNewArticle: boolean | undefined;
    article: API.ArticleDto;
    topic: API.TopicDto;
    topicList: API.TopicDto[];
    tagList: API.TagDto[];
}

const ArticleCard = (props: Props) => {
    const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);

    const { isNewArticle, topic, topicList, tagList } = props;
    const [article, setArticle] = useState(props.article);
    const parentArticleList = topic.articles;
    const [isEditing, setIsEditing] = useState(false);
    const [editedContent, setEditedContent] = useState(article.content);
    const [editedAbstraction, setEditedAbstraction] = useState(article.abstraction);
    const [isSaving, setIsSaving] = useState(false);
    const isTopic = article.articleType === "TopicArticle";

    
    const handleSave = async () => {
        setIsSaving(true);
        const hide = message.loading('Updating...');

        try {
            await apiArticleEditByRequest({
                articleId: article.articleId,
                content: editedContent,
                abstraction: editedAbstraction
            });
            hide();
            setIsEditing(false);
            message.success('Update successful!');
            window.location.reload();
        } catch (error: any) {
            hide();
            message.error('Update failed: ' + error.message);
        } finally {
            setIsSaving(false);
        }
    };
    

    const onMenuClick: MenuProps['onClick'] = (e) => {
        if (e.key === '1') { setUpdateModalVisible(true); }
    };

    const items = [
        {
            key: '1',
            label: 'Modify Attributes',
        },
        {
            key: '2',
            label: 'Download',

        },
    ];
    const handleCancel = () => {
        setEditedAbstraction(article.abstraction);
        setEditedContent(article.content);
        setIsEditing(false);
    };
    useEffect(() => {
        console.log(article);
        if(isNewArticle){
            setUpdateModalVisible(true);
        }
    }, [isNewArticle]);

    return (
        <div className="article-card">
            <Card className="header-card">
                <Flex justify="flex-start">
                    <Title style={{ fontSize: 24, flexGrow: 1, marginTop: 0 }}>
                        {isTopic?"Topic: ":""}{article.title}
                    </Title>

                    {
                        (isNewArticle || isTopic) ? (
                            <></>
                        ) : (
                            <Flex gap={12}>
                                {isEditing && (
                                    <>
                                        <Button color="default" variant="filled" style={{ width: 100 }} onClick={handleCancel}>
                                            Cancel
                                        </Button>
                                        <Button color="default" variant="solid" style={{ width: 100 }} onClick={handleSave}>
                                            Save
                                        </Button>
                                    </>
                                )}
                                {!isEditing && (
                                    <Dropdown.Button menu={{ items, onClick: onMenuClick }} onClick={() => setIsEditing(true)} style={{ width: 100 }}>
                                        Edit
                                    </Dropdown.Button>
                                )}
                            </Flex>)
                    }

                </Flex>
                <TagList tagList={article.tags} />
                <div style={{ marginBottom: 16 }} />
                {isEditing ? (
                    <TextArea
                        showCount
                        maxLength={300}
                        value={editedAbstraction}
                        onChange={(e) => setEditedAbstraction(e.target.value)}
                        style={{ height: 80, resize: "none" }}
                    />
                ) : (
                    <MdViewer value={article.abstraction} />
                )}
            </Card>
            <div style={{ marginBottom: 16 }} />
            <Card className="content-card">
                {isEditing ? (
                    <MdEditor
                        value={editedContent}
                        onChange={(value) => setEditedContent(value)}
                    />
                ) : (
                    <MdViewer value={article.content} />
                )}
            </Card>
            <ArticleFormModal
                isCreate={isNewArticle}
                parentArticleList={parentArticleList}
                topicList={topicList}
                tagList={tagList}
                currentArticle={article}
                visible={updateModalVisible}
                onSubmit={() => { setUpdateModalVisible(false); window.location.reload(); }}
                onCancel={() => {
                    setUpdateModalVisible(false);
                }}
            />

        </div>
    );
};

export default ArticleCard;
