import TopicsPage from './TopicsPage';

export const dynamic = 'force-dynamic'; // Options: 'auto' | 'force-dynamic' | 'error' | 'force-static'

export default function Page() {
  return <TopicsPage />;
}

export const metadata = {
  title: 'Topics - Firefly Articles',
  description: 'Browse all topics by category in Firefly Articles'
}; 