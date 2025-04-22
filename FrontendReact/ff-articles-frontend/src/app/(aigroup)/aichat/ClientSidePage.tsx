import { ChatProvider } from './context/ChatContext';
import AiChatPage from './AiChatPage';

export default function ClientSidePage() {
    return (
        <ChatProvider>
            <AiChatPage />
        </ChatProvider>
    );
} 