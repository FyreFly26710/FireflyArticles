"use client";
import { setLoginUser } from "@/stores/loginUser";
import { storage } from "@/stores/storage";
import { useEffect, useCallback } from "react";
import { useDispatch } from "react-redux";
import { AppDispatch } from "@/stores/reduxStore";
import { apiAuthGetLoginUser } from "@/api/identity/api/auth";

const UserStateLayout: React.FC<Readonly<{ children: React.ReactNode; }>> = ({ children }) => {
    const dispatch = useDispatch<AppDispatch>();

    const doInitLoginUser = useCallback(async () => {
        try {
            let user = storage.getUser();
            // if user is not in localStorage, get user from backend
            // Todo: use guest user if user is not in localStorage
            if (!user) {
                const res = await apiAuthGetLoginUser();
                user = res.data ?? null;

                if (user) {
                    storage.setUser(user);
                }
            }

            if (user) {
                dispatch(setLoginUser(user));
            }
        } catch (error: any) {
            console.error('Failed to initialize user:', error);
        }
    }, []);

    useEffect(() => {
        doInitLoginUser();
    }, []);

    return <>{children}</>;
};

export default UserStateLayout;