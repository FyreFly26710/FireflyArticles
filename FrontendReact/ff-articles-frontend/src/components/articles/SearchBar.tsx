import { useState } from 'react';
import { Input, Button, Space } from 'antd';
import { SearchOutlined, CloseCircleOutlined } from '@ant-design/icons';

interface SearchBarProps {
  onSearch: (keyword: string) => void;
  onClear: () => void;
  initialValue?: string;
}

const SearchBar = ({ onSearch, onClear, initialValue = '' }: SearchBarProps) => {
  const [keyword, setKeyword] = useState(initialValue);

  const handleSearch = () => {
    onSearch(keyword.trim());
  };

  const handleClear = () => {
    setKeyword('');
    onClear();
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      handleSearch();
    }
  };

  return (
    <Space.Compact className="w-full h-full flex items-center">
      <Input
        placeholder="Search articles and descriptions..."
        value={keyword}
        onChange={(e) => setKeyword(e.target.value)}
        onKeyDown={handleKeyDown}
        suffix={
          keyword ? (
            <CloseCircleOutlined 
              className="text-gray-400 cursor-pointer" 
              onClick={handleClear} 
            />
          ) : null
        }
        allowClear
      />
      <Button 
        type="primary" 
        icon={<SearchOutlined />} 
        onClick={handleSearch}
      >
        Search
      </Button>
    </Space.Compact>
  );
};

export default SearchBar; 