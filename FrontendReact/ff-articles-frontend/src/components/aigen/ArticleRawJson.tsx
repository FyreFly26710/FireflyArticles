'use client';

import React from 'react';
import { Typography, Input, Button, Space } from 'antd';
import { SyncOutlined } from '@ant-design/icons';

const { Text } = Typography;
const { TextArea } = Input;

interface ArticleRawJsonProps {
    responseData: string;
    onDataChange: (data: string) => void;
    onTryParse: () => void;
}

const ArticleRawJson: React.FC<ArticleRawJsonProps> = ({
    responseData,
    onDataChange,
    onTryParse
}) => {
    return (
        <Space direction="vertical" style={{ width: '100%' }} size="large">
            <div>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 }}>
                    <Text strong>Response Data:</Text>
                    <Button
                        type="primary"
                        icon={<SyncOutlined />}
                        onClick={onTryParse}
                    >
                        Try Parse
                    </Button>
                </div>
                <TextArea
                    value={responseData}
                    onChange={(e) => onDataChange(e.target.value)}
                    autoSize={{ minRows: 10, maxRows: 50 }}
                    style={{ fontFamily: 'monospace' }}
                />
            </div>
        </Space>
    );
};

export default ArticleRawJson; 