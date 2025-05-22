import dynamic from 'next/dynamic'

const AiGenPage = dynamic(
  () => import('./AiGenPage'),
  { ssr: false }
);

export default function Page() {
  return <AiGenPage />;
}