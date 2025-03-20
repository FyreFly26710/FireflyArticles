"use client";
import { Avatar, Card, List, Typography } from "antd";
import Link from "next/link";

interface Props {
  topicList: API.TopicDto[];
}

const TopicList = (props: Props) => {
  const { topicList = [] } = props;

  return (
    <div className="topic-list">
      <List
        grid={{ gutter: 16, column: 4, xs: 1, sm: 2, md: 3, lg: 3, }}
        dataSource={topicList}
        renderItem={(item: API.TopicDto) => (
          <List.Item>
            <Card>
              <Link href={`/topic/${item.topicId}`}>
                <Card.Meta
                  avatar={<Avatar src={item.topicImage} />}
                  title={item.title}
                  description={
                    <Typography.Paragraph
                      type="secondary"
                      ellipsis={{ rows: 1 }}
                      style={{ marginBottom: 0 }}
                    >
                      {item.abstract}
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
