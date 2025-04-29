import { Tag } from "antd";
import { CheckCircleFilled } from '@ant-design/icons';
import styled from "styled-components";

interface Props {
    tagData?: API.TagDto[];
    selectedTags?: string[];
    onSelectionChange?: (selectedTags: string[]) => void;
}

const TagListContainer = styled.div`
    display: flex;
    flex-wrap: wrap;
    gap: 8px;

    .ant-tag {
        margin-right: 0;
        transition: all 0.3s;
    }

    .ant-tag:hover {
        opacity: 0.8;
    }
`;

const TagSelect = (props: Props) => {
    const {
        tagData = [],
        selectedTags = [],
        onSelectionChange
    } = props;

    const handleTagClick = (tagName: string) => {
        if (!onSelectionChange) return;

        const newSelectedTags = selectedTags.includes(tagName)
            ? selectedTags.filter(tag => tag !== tagName)
            : [...selectedTags, tagName];

        onSelectionChange(newSelectedTags);
    };

    return (
        <TagListContainer>
            {tagData.map((tag) => {
                const isSelected = selectedTags.includes(tag.tagName!);
                return (
                    <Tag
                        key={tag.tagName}
                        onClick={() => handleTagClick(tag.tagName!)}
                        style={{
                            cursor: 'pointer',
                            backgroundColor: isSelected ? '#e6f7ff' : undefined,
                            borderColor: isSelected ? '#1890ff' : undefined,
                        }}
                    >
                        {tag.tagName}
                        {isSelected && (
                            <CheckCircleFilled
                                style={{
                                    marginLeft: 5,
                                    color: '#1890ff'
                                }}
                            />
                        )}
                    </Tag>
                );
            })}
        </TagListContainer>
    );
}

export default TagSelect; 