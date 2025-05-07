"use client";

import { Row, Col, Skeleton } from 'antd';

export default function ArticleLoading() {
  return (
    <div className="p-4 md:p-6">
      <Row gutter={[16, 16]} style={{ margin: '0px' }}>
        <Col xs={24} lg={24}>
          <div className="bg-white rounded-lg shadow-sm p-6 mb-4">
            <Skeleton active avatar paragraph={{ rows: 2 }} />
          </div>
        </Col>
        <Col xs={24} lg={24}>
          <div className="bg-white rounded-lg shadow-sm p-6">
            <Skeleton active paragraph={{ rows: 12 }} />
          </div>
        </Col>
      </Row>
    </div>
  );
} 