import dynamic from 'next/dynamic'

// Import entire page with ChatProvider dynamically with SSR disabled
const AiChatPage = dynamic(
  () => import('./AiChatPage'),
  { ssr: false }
);

export default function Page() {
  return <AiChatPage />;
}