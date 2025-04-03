const STORAGE_KEY_USER = 'user-storage';

export const storage = {
    getUser: (): API.LoginUserDto | null => {
        try {
            const storedUser = localStorage.getItem(STORAGE_KEY_USER);
            if (!storedUser) return null;

            const parsedUser = JSON.parse(storedUser);
            return parsedUser?.user || null;
        } catch (e) {
            // If there's any error, clear the storage and return null
            localStorage.removeItem(STORAGE_KEY_USER);
            return null;
        }
    },

    setUser: (user: API.LoginUserDto | null): void => {
        if (user) {
            localStorage.setItem(STORAGE_KEY_USER, JSON.stringify({ user }));
        } else {
            localStorage.removeItem(STORAGE_KEY_USER);
        }
    },

    clearUser: (): void => {
        localStorage.removeItem(STORAGE_KEY_USER);
    }
}; 