import { AiGenProvider } from '../../../stores/aiGenContext';
import AiGenPage from './AiGenPage';

export default function ClientSidePage() {
    return (
        <AiGenProvider>
            <AiGenPage />
        </AiGenProvider>
    );
} 