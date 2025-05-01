import { Select, Tag } from "antd";
import { PlusOutlined } from '@ant-design/icons';
import { useEffect, useState } from "react";

interface TagSelectProps {
    tags: API.TagDto[];
    selectedTags: string[];
    onChange: (selectedTags: string[]) => void;
}

/**
 * A tag selection component built on Ant Design's Select
 * Allows selecting from existing tags or creating new ones
 */
const TagSelect = ({ tags, selectedTags, onChange }: TagSelectProps) => {
    const [inputValue, setInputValue] = useState('');
    const [internalValue, setInternalValue] = useState<string[]>(selectedTags);

    // Sync internal state with external selected tags
    useEffect(() => {
        setInternalValue(selectedTags);
    }, [selectedTags]);

    // Transform tag objects to select options
    const tagOptions = tags.map(tag => ({
        value: tag.tagName || '',
        label: tag.tagName || '',
    }));

    // Handle tag selection change
    const handleChange = (values: string[]) => {
        setInternalValue(values);
        onChange(values);
    };

    // Handle creating a new tag
    const handleAddTag = () => {
        const trimmedInput = inputValue.trim();
        if (!trimmedInput) return;

        // Check if tag already exists
        const tagExists = internalValue.includes(trimmedInput) ||
            tags.some(tag => tag.tagName === trimmedInput);

        if (!tagExists) {
            const newSelectedTags = [...internalValue, trimmedInput];
            setInternalValue(newSelectedTags);
            onChange(newSelectedTags);
        }

        setInputValue('');
    };

    // Custom dropdown render to add "Add new tag" option
    const dropdownRender = (menu: React.ReactElement) => (
        <>
            {menu}
            {inputValue && (
                <div
                    style={{
                        display: 'flex',
                        alignItems: 'center',
                        padding: '8px 12px',
                        cursor: 'pointer',
                        borderTop: '1px solid #f0f0f0'
                    }}
                    onClick={handleAddTag}
                    onMouseDown={e => e.preventDefault()}
                >
                    <PlusOutlined style={{ marginRight: 8 }} />
                    <span>Add tag: {inputValue}</span>
                </div>
            )}
        </>
    );

    return (
        <Select
            mode="multiple"
            style={{ width: '100%' }}
            placeholder="Select or create tags"
            value={internalValue}
            onChange={handleChange}
            options={tagOptions}
            dropdownRender={dropdownRender}
            onSearch={setInputValue}
            searchValue={inputValue}
            optionFilterProp="label"
            tagRender={props => (
                <Tag
                    color="blue"
                    closable={props.closable}
                    onClose={props.onClose}
                    style={{ marginRight: 3 }}
                >
                    {props.label}
                </Tag>
            )}
        />
    );
};

export default TagSelect; 