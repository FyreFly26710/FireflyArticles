import { ChatProvider } from './context/ChatContext';
import AiChatPage from './AiChatPage';

export default function Page() {
  return (
    <ChatProvider>
      <AiChatPage />
    </ChatProvider>
  );
}