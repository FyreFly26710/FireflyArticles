import AiGenPage from './AiGenPage';
import { AiGenProvider } from '@/stores/AiGenContext';
export default function ClientSidePage() {
    return (
        <AiGenProvider>
            <AiGenPage />
        </AiGenProvider>
    );
} 