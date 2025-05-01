import AiGenPage from './AiGenPage';
import { AiGenProvider } from '@/states/AiGenContext';
export default function ClientSidePage() {
    return (
        <AiGenProvider>
            <AiGenPage />
        </AiGenProvider>
    );
} 