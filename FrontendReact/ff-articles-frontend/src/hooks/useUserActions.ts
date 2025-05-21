import { useCallback } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useRouter } from "next/navigation";
import { message } from 'antd';
import { RootState } from '@/stores';
import { setLoginUser } from '@/stores/loginUserSlice';
import { apiAuthLogout, apiAuthGetLoginUser } from '@/api/identity/api/auth';
import { DEFAULT_USER } from '@/libs/constants/user';

export const useUserActions = () => {
    const dispatch = useDispatch();
    const router = useRouter();
    const loginUser = useSelector((state: RootState) => state.loginUser);

    const initializeUser = useCallback(async () => {
        try {
            const response = await apiAuthGetLoginUser();
            const user = response.data ?? null;
            
            if (user) {
                dispatch(setLoginUser(user));
            }
        } catch (error) {
            console.error('Failed to initialize user:', error);
        }
    }, [dispatch]);

    const updateUser = useCallback((user: API.LoginUserDto) => {
        dispatch(setLoginUser(user));
    }, [dispatch]);

    const clearUser = useCallback(() => {
        dispatch(setLoginUser(DEFAULT_USER));
    }, [dispatch]);

    const handleLogout = useCallback(async () => {
        try {
            await apiAuthLogout();
            message.success("Logout successfully");
            clearUser();
            router.push("/user/login");
        } catch (error) {
            console.error('Error during logout:', error);
            message.error(error instanceof Error ? error.message : 'Failed to logout');
        }
    }, [clearUser, router]);

    return {
        // State
        loginUser,

        // Actions
        initializeUser,
        updateUser,
        clearUser,
        handleLogout,
    };
}; 