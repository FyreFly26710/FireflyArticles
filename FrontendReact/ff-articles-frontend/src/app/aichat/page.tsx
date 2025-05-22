import dynamic from 'next/dynamic'

const AiChatPage = dynamic(
  () => import('./AiChatPage'),
  { ssr: false }
);

export default function Page() {
  return <AiChatPage />;
}