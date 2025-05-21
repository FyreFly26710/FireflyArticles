import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { DEFAULT_USER } from "@/libs/constants/user";

export const loginUserSlice = createSlice({
    name: "loginUser",
    initialState: DEFAULT_USER,
    reducers: {
        setLoginUser: (state, action: PayloadAction<API.LoginUserDto>) => {
            return {
                ...action.payload,
            };
        },
    },
});

export const { setLoginUser } = loginUserSlice.actions; 