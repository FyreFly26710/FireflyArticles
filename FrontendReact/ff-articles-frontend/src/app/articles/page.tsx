import ArticlesPage from './ArticlesPage';

export const dynamic = 'force-dynamic'; // Options: 'auto' | 'force-dynamic' | 'error' | 'force-static'

export default function Page() {
  return <ArticlesPage />;
}

export const metadata = {
  title: 'Articles - Firefly Articles',
  description: 'Browse all articles in Firefly Articles'
}; 