import { AiGenProvider } from './context/AiGenContext';
import AiGenPage from './AiGenPage';

export default function Page() {
  return (
    <AiGenProvider>
      <AiGenPage />
    </AiGenProvider>
  );
}