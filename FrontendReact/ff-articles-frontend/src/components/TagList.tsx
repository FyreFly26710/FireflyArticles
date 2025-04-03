import { Tag } from "antd";

interface Props {
    tags?: string[];
}

const TagList = (props: Props) => {
    const { tags = [] } = props;

    return (
        <div className="tag-list">
            {tags.map((tag) => (
                <Tag key={tag}>{tag}</Tag>
            ))}
        </div>
    );
};

export default TagList; 