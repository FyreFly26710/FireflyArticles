import dynamic from 'next/dynamic'

// Import entire page with ChatProvider dynamically with SSR disabled
const ClientSidePage = dynamic(
  () => import('./ClientSidePage'),
  { ssr: false }
);

export default function Page() {
  return <ClientSidePage />;
}