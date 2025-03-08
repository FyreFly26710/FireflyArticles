import { Tag } from "antd";
import { CheckCircleFilled } from '@ant-design/icons';
import "./index.css";

interface Props {
    tagData?: API.TagDto[];
    selectedTagIds?: number[];
    onSelectionChange?: (selectedIds: number[]) => void;
}

const TagSelect = (props: Props) => {
    const {
        tagData = [],
        selectedTagIds = [],
        onSelectionChange
    } = props;

    const handleTagClick = (tagId: number) => {
        if (!onSelectionChange) return;

        const newSelectedIds = selectedTagIds.includes(tagId)
            ? selectedTagIds.filter(id => id !== tagId)
            : [...selectedTagIds, tagId];

        onSelectionChange(newSelectedIds);
    };

    return (
        <div className="tag-list">
            {tagData.map((tag) => {
                const isSelected = selectedTagIds.includes(tag.tagId!);
                return (
                    <Tag
                        key={tag.tagId}
                        onClick={() => handleTagClick(tag.tagId!)}
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
        </div>
    );
}


export default TagSelect;