import { ChatProvider } from '../../../stores/aiChatContext';
import AiChatPage from './AiChatPage';

export default function ClientSidePage() {
    return (
        <ChatProvider>
            <AiChatPage />
        </ChatProvider>
    );
} 