import { Typography, Card, Avatar } from "antd";
import Link from "next/link";

interface TopicCardProps {
    topic: API.TopicDto;
}

const TopicCard = (props: TopicCardProps) => {
    const { topic } = props;

    return (
        <Card>
            <Link href={`/topic/${topic.topicId}`}>
                <Card.Meta
                    avatar={<Avatar src={topic.topicImage} />}
                    title={topic.title}
                    description={
                        <Typography.Paragraph
                            type="secondary"
                            ellipsis={{ rows: 1 }}
                            style={{ marginBottom: 0 }}
                        >
                            {topic.abstract}
                        </Typography.Paragraph>
                    }
                />
            </Link>
        </Card>
    );
};

export default TopicCard;