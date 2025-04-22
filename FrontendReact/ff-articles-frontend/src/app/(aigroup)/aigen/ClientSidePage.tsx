import { AiGenProvider } from './context/AiGenContext';
import AiGenPage from './AiGenPage';

export default function ClientSidePage() {
    return (
        <AiGenProvider>
            <AiGenPage />
        </AiGenProvider>
    );
} 