import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import {DEFAULT_USER} from "@/constants/user";

/**
 * Slice: from redux toolkit to simplify redux
 * Global state for the logged-in user
 */
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

// Modify state
export const { setLoginUser } = loginUserSlice.actions;

export default loginUserSlice.reducer;
