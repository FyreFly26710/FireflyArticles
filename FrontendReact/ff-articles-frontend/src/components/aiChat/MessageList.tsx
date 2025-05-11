import { useEffect, useRef, useState } from 'react'
import { useChat } from '@/states/ChatContext'
import { storage, ChatDisplaySettings } from '@/states/localStorage'
import Message from './Message'

export default function MessageList() {
    const ref = useRef<HTMLDivElement>(null)
    const { session } = useChat()
    const rounds = session.rounds

    // Initialize display settings with state
    const [displaySettings, setDisplaySettings] = useState(() => storage.getChatDisplaySettings());

    // Listen for display settings changes
    useEffect(() => {
        const handleDisplayChange = (event: CustomEvent<ChatDisplaySettings>) => {
            setDisplaySettings(event.detail);
        };

        // Handle changes from other tabs
        const handleStorageChange = (event: StorageEvent) => {
            if (event.key === 'chat-display-settings') {
                setDisplaySettings(storage.getChatDisplaySettings());
            }
        };

        window.addEventListener('chatDisplaySettingsChanged', handleDisplayChange);
        window.addEventListener('storage', handleStorageChange);

        return () => {
            window.removeEventListener('chatDisplaySettingsChanged', handleDisplayChange);
            window.removeEventListener('storage', handleStorageChange);
        };
    }, []);

    // Filter messages if showOnlyActiveMessages is true
    const filteredRounds = displaySettings.showOnlyActiveMessages
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
