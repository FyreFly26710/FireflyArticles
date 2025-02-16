"use client";
import { Avatar, Card, List, Typography } from "antd";
import "./index.css";
import Link from "next/link";

interface Props {
  topicList: API.TopicResponse[];
}

const TopicList = (props: Props) => {
  const { topicList = [] } = props;

  return (
    <div className="topic-list">
      <List
        grid={{gutter: 16, column: 4, xs: 1, sm: 2, md: 3, lg: 3, }}
        dataSource={topicList}
        renderItem={(item: API.TopicResponse) => (
          <List.Item>
            <Card>
              <Link href={`/bank/${item.topicId}`}>
                <Card.Meta
                  avatar={<Avatar src={item.topicImage} />}
                  title={item.title}
                  description={
                    <Typography.Paragraph
                      type="secondary"
                      ellipsis={{ rows: 1 }}
                      style={{ marginBottom: 0 }}
                    >
                      {item.abstraction}
                    </Typography.Paragraph>
                  }
                />
              </Link>
            </Card>
          </List.Item>
        )}
      />
    </div>
  );
};

export default TopicList;
