import { useDispatch, useSelector } from 'react-redux';
import { useRouter } from "next/navigation";
import { message } from 'antd';
import { RootState } from '@/stores';
import { setLoginUser } from '@/stores/loginUserSlice';
import { apiAuthLogout, apiAuthGetLoginUser, apiAuthLogin, apiAuthRegister } from '@/api/identity/api/auth';
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

    const handleLogin = async (values: API.UserLoginRequest) => {
        try {
            const res = await apiAuthLogin(values);
            if (res.data) {
                message.success("Login successful!");
                dispatch(setLoginUser(res.data));
                router.replace("/");
                return true;
            }
            return false;
        } catch (error: any) {
            message.error('Login failed, ' + error.message);
            return false;
        }
    };

    const handleRegister = async (values: API.UserRegisterRequest) => {
        try {
            const res = await apiAuthRegister(values);
            if (res.code === 200) {
                message.success("Registration successful, please log in");
                router.push("/user/login");
                return true;
            } else {
                message.error("Registration failed, " + res.message);
                return false;
            }
        } catch (error: any) {
            message.error("Registration failed, " + error.message);
            return false;
        }
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

    const isLoggedIn = () => {
        return loginUser?.id !== undefined && loginUser?.id !== null;
    };

    const isAdmin = () => {
        return loginUser?.userRole === 'admin';
    };

    return {
        // State
        loginUser,

        // Status checks
        isLoggedIn,
        isAdmin,

        // Actions
        initializeUser,
        updateUser,
        clearUser,
        handleLogin,
        handleRegister,
        handleLogout,
    };
}; 