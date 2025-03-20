"use client";

import { useEffect, useState } from "react";
import { Avatar, Button, Card, Dropdown, Flex, Input, MenuProps, message } from "antd";
import Title from "antd/es/typography/Title";
import "./index.css";
import { apiArticleAddByRequest, apiArticleEditByRequest } from "@/api/contents/api/article";
import TagList from "@/components/TagList";
import MdEditor from "@/components/MdEditor";
import ArticleFormModal from "@/components/ArticleFormModal";
import { useRouter } from "next/navigation";

import dynamic from 'next/dynamic'

const MdViewer = dynamic(() => import('@/components/MdViewer'), {
    ssr: false
})

const { TextArea } = Input;

interface Props {
    isNewArticle?: boolean | undefined;
    redirectArticleId?: number | undefined;
    article: API.ArticleDto;
    topic: API.TopicDto | undefined;
    topicList: API.TopicDto[];
    tagList: API.TagDto[];
}

const ArticleCard = (props: Props) => {
    const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);

    const { isNewArticle, topic, article, topicList, tagList, redirectArticleId } = props;
    const parentArticleList = topic?.articles;
    const [isEditing, setIsEditing] = useState(false);
    const [editedTitle, setEditedTitle] = useState(article.title);
    const [editedContent, setEditedContent] = useState(article.content);
    const [editedAbstract, setEditedAbstract] = useState(article.abstract);
    const [isSaving, setIsSaving] = useState(false);
    const isTopic = article.articleType === "TopicArticle";
    const router = useRouter();

    const handleSave = async () => {
        setIsSaving(true);
        const hide = message.loading('Updating...');

        try {
            if (isNewArticle) {
                const res = await apiArticleAddByRequest({
                    ...article,
                    title: editedTitle,
                    content: editedContent,
                    abstract: editedAbstract
                })
                //@ts-ignore
                const id: number = res.data
                hide();
                setIsEditing(false);
                message.success('Update successful!');
                router.push(`/topic/${topic?.topicId}/article/${id}`);
            }
            if (!isNewArticle) {
                await apiArticleEditByRequest({
                    articleId: article.articleId,
                    title: editedTitle,
                    content: editedContent,
                    abstract: editedAbstract
                });
                hide();
                setIsEditing(false);
                message.success('Update successful!');
                window.location.reload();
            }
        } catch (error: any) {
            hide();
            console.log(error)
            message.error('Update failed');
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
        setEditedTitle(article.title);
        setEditedAbstract(article.abstract);
        setEditedContent(article.content);
        setIsEditing(false);
        if (isNewArticle) {
            const url = String(redirectArticleId) === String(0)
                ? `/topic/${topic?.topicId}`
                : `/topic/${topic?.topicId}/article/${redirectArticleId}`;
            router.push(url);
        }
    };
    useEffect(() => {
        if (isNewArticle) {
            setIsEditing(true);
        }
    }, [isNewArticle]);

    return (
        <div className="article-card">
            <Card className="header-card">
                <Flex justify="space-between">
                    <div style={{ paddingLeft: 20, width: '100%' }}>
                        {isEditing ? (
                            <Input placeholder="Please enter a title"
                                variant="underlined"
                                value={editedTitle}
                                onChange={(e) => setEditedTitle(e.target.value)}
                                style={{ fontSize: 20, flexGrow: 1, marginTop: 0 }}
                            />
                        ) : (
                            <Title style={{ fontSize: 24, flexGrow: 1, marginTop: 0 }}>
                                {topic && <Avatar src={topic.topicImage} size={24} style={{ marginRight: 8, marginTop: 0 }} />}
                                {isTopic ? "Topic: " : ""}{article.title}
                            </Title>
                        )}
                    </div>
                    <div>

                        {
                            !isTopic &&
                            <Flex gap={12}>
                                {isEditing && (
                                    <>
                                        <Button color="default" variant="filled" style={{ width: 80 }} onClick={handleCancel}>
                                            Cancel
                                        </Button>
                                        <Button color="primary" variant="solid" style={{ width: 80 }} onClick={handleSave}>
                                            Save
                                        </Button>
                                    </>
                                )}
                                {!isEditing && (
                                    <Dropdown.Button menu={{ items, onClick: onMenuClick }} onClick={() => setIsEditing(true)} style={{ width: 100 }}>
                                        Edit
                                    </Dropdown.Button>
                                )}
                            </Flex>
                        }
                    </div>

                </Flex>
                {!isEditing && <TagList tagList={article.tags} />}
                <div style={{ marginBottom: 12 }} />
                {isEditing ? (
                    <TextArea
                        showCount
                        value={editedAbstract}
                        onChange={(e) => setEditedAbstract(e.target.value)}
                        style={{ height: 80, resize: "none" }}
                    />
                ) : (
                    <MdViewer value={article.abstract} />
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
                parentArticleList={parentArticleList}
                tagList={tagList}
                modifyContent={true}
                currentArticle={article}
                visible={updateModalVisible}
                onSubmit={() => { setUpdateModalVisible(false); window.location.reload(); }}
                onCancel={() => { setUpdateModalVisible(false); }}
            />

        </div>
    );
};

export default ArticleCard;
