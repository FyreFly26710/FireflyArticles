import { useEffect, useRef } from 'react'
import { useChat } from '@/hooks/useChat'
import { useSettings } from '@/hooks/useSettings'
import Message from './Message'

export default function MessageList() {
    const ref = useRef<HTMLDivElement>(null)
    const { session } = useChat()
    const { settings } = useSettings();
    const rounds = session.rounds

    // Filter messages if showOnlyActiveMessages is true
    const filteredRounds = settings.chatDisplay.showOnlyActiveMessages
        ? rounds?.filter(round => round.isActive)
        : rounds;

    // Handle hash change for anchor links
    useEffect(() => {
        const handleHashChange = () => {
            const hash = window.location.hash;
            if (hash && ref.current) {
                const targetId = hash.replace('#', '');
                const element = document.getElementById(targetId);
                if (element) {
                    // Give a little time for any animations or renders to complete
                    setTimeout(() => {
                        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
                    }, 100);
                }
            }
        };

        // Check hash on initial load
        if (window.location.hash) {
            handleHashChange();
        }

        // Listen for hash changes
        window.addEventListener('hashchange', handleHashChange);
        return () => {
            window.removeEventListener('hashchange', handleHashChange);
        };
    }, []);

    return (
        <div
            className="w-full h-full py-4 overflow-y-auto bg-white scroll-smooth message-list-container"
            ref={ref}
            style={{
                overscrollBehavior: 'contain',
                paddingBottom: '140px', // Ensure space at bottom for input box
                msOverflowStyle: 'none', // Hide scrollbar in IE and Edge
                scrollbarWidth: 'none', // Hide scrollbar in Firefox
            }}
        >
            {filteredRounds && filteredRounds.length > 0 ? (
                filteredRounds.map((chatRound) => (
                    <div
                        key={`${chatRound.sessionId}-${chatRound.chatRoundId}`}
                        id={`chat-round-${chatRound.chatRoundId}`}
                        className="scroll-mt-16 mb-1"
                    >
                        <Message
                            chatRound={chatRound}
                            collapseThreshold={150}
                        />
                    </div>
                ))
            ) : (
                <div className="flex items-center justify-center h-full">
                    <div className="text-center text-gray-400">
                        <p className="text-lg">No messages yet</p>
                        <p className="text-sm">Start a conversation below</p>
                    </div>
                </div>
            )}
        </div>
    )
}
