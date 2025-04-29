import { ChatProvider } from '../../stores/ChatContext';
import AiChatPage from './AiChatPage';

export default function ClientSidePage() {
    return (
        <ChatProvider>
            <AiChatPage />
        </ChatProvider>
    );
} 