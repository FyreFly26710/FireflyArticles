"use client";
import { List } from "antd";
import TopicCard from "../shared/TopicCard";

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
            <TopicCard topic={item} />
          </List.Item>
        )}
      />
    </div>
  );
};

export default TopicList; 