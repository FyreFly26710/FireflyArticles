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

    const initializeUser = async () => {
        try {
            const response = await apiAuthGetLoginUser();
            const user = response.data ?? null;

            if (user) {
                dispatch(setLoginUser(user));
            }
        } catch (error) {
            console.error('Failed to initialize user:', error);
        }
    };

    const updateUser = (user: API.LoginUserDto) => {
        dispatch(setLoginUser(user));
    };

    const clearUser = () => {
        dispatch(setLoginUser(DEFAULT_USER));
    };

    const handleLogout = async () => {
        try {
            await apiAuthLogout();
            message.success("Logout successfully");
            clearUser();
            router.push("/user/login");
        } catch (error) {
            console.error('Error during logout:', error);
            message.error(error instanceof Error ? error.message : 'Failed to logout');
        }
    };

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