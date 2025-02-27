"use client";

import { useState } from "react";
import { Button, Card, Dropdown, Flex, Input, MenuProps } from "antd";
import Title from "antd/es/typography/Title";
import MdViewer from "../MdViewer";
import MdEditor from "../MdEditor";
import TagList from "../TagList";
import "./index.css";

const { TextArea } = Input;

interface Props {
    article: API.ArticleDto;
}

const ArticleCard = (props: Props) => {
    const { article } = props;

    // State for edit mode and storing updates
    const [isEditing, setIsEditing] = useState(false);
    const [editedContent, setEditedContent] = useState(article.content);
    const [editedAbstraction, setEditedAbstraction] = useState(article.abstraction);
    const [isSaving, setIsSaving] = useState(false);

    // Handle abstraction text change
    const handleAbstractionChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
        setEditedAbstraction(e.target.value);
    };

    // Handle content change in markdown editor
    const handleContentChange = (newContent: string) => {
        setEditedContent(newContent);
    };

    // Save function (replace with API call)
    const handleSave = async () => {
        setIsSaving(true);

        try {
            // TODO: Call API to save changes
            console.log("Saving:", { editedAbstraction, editedContent });

            // Simulate API call delay
            await new Promise((resolve) => setTimeout(resolve, 1000));

            // Exit edit mode after saving
            setIsEditing(false);
        } catch (error) {
            console.error("Failed to save:", error);
        } finally {
            setIsSaving(false);
        }
    };

    const onMenuClick: MenuProps['onClick'] = (e) => {
        console.log('click', e);
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
    

    return (
        <div className="article-card">
            <Card className="header-card">
                <Flex justify="flex-start">
                    <Title style={{ fontSize: 24, flexGrow: 1, marginTop: 0 }}>
                        {article.title}
                    </Title>

                    <Flex gap={12}>
                        {isEditing && (
                            <>
                                <Button color="default" variant="filled" style={{width: 100}}
                                    onClick={handleCancel}>Cancel</Button>
                                <Button color="default" variant="solid" style={{width: 100}}
                                    onClick={handleSave}>Save</Button>
                            </>
                        )}
                        {!isEditing && (
                            <Dropdown.Button menu={{ items, onClick: onMenuClick }}
                                onClick={() => setIsEditing(true)} style={{width: 100}}
                            >
                                Edit
                            </Dropdown.Button>
                        )}
                    </Flex>
                </Flex>
                <TagList tagList={article.tags} />
                <div style={{ marginBottom: 16 }} />
                {isEditing ? (
                    <TextArea
                        showCount
                        maxLength={300}
                        value={editedAbstraction}
                        onChange={handleAbstractionChange}
                        placeholder="Edit abstraction..."
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
                        onChange={handleContentChange}
                        placeholder="Edit article content..."
                    />
                ) : (
                    <MdViewer value={article.content} />
                )}
            </Card>
        </div>
    );
};

export default ArticleCard;
