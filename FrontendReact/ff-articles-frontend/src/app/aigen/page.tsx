import dynamic from 'next/dynamic'

// Import entire page with ChatProvider dynamically with SSR disabled
const AiGenPage = dynamic(
  () => import('./AiGenPage'),
  { ssr: false }
);

export default function Page() {
  return <AiGenPage />;
}