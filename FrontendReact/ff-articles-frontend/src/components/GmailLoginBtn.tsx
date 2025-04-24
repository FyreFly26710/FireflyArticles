import React from 'react';
import { Button, message } from 'antd';
import styled from "styled-components";
import { setLoginUser } from '@/stores/loginUser';
import { storage } from '@/stores/storage';
import { apiAuthGetLoginUser } from '@/api/identity/api/auth';
import { AppDispatch } from "@/stores";
import { useDispatch } from "react-redux";


const GoogleIcon = () => (
    <svg width="18" height="18" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 48 48">
        <path fill="#EA4335" d="M24 9.5c3.54 0 6.71 1.22 9.21 3.6l6.85-6.85C35.9 2.38 30.47 0 24 0 14.62 0 6.51 5.38 2.56 13.22l7.98 6.19C12.43 13.72 17.74 9.5 24 9.5z" />
        <path fill="#4285F4" d="M46.98 24.55c0-1.57-.15-3.09-.38-4.55H24v9.02h12.94c-.58 2.96-2.26 5.48-4.78 7.18l7.73 6c4.51-4.18 7.09-10.36 7.09-17.65z" />
        <path fill="#FBBC05" d="M10.53 28.59c-.48-1.45-.76-2.99-.76-4.59s.27-3.14.76-4.59l-7.98-6.19C.92 16.46 0 20.12 0 24c0 3.88.92 7.54 2.56 10.78l7.97-6.19z" />
        <path fill="#34A853" d="M24 48c6.48 0 11.93-2.13 15.89-5.81l-7.73-6c-2.15 1.45-4.92 2.3-8.16 2.3-6.26 0-11.57-4.22-13.47-9.91l-7.98 6.19C6.51 42.62 14.62 48 24 48z" />
    </svg>
);

const StyledButton = styled(Button)`
    height: 40px !important;
    display: flex !important;
    align-items: center !important;
    justify-content: center !important;
    gap: 8px !important;
    background-color: #fff !important;
    border: 1px solid #ddd !important;

    &:hover {
        background-color: #f8f8f8 !important;
        border-color: #c6c6c6 !important;
    }
`;

const GoogleLoginButton = () => {
    const dispatch = useDispatch<AppDispatch>();
    const apiUrl = process.env.NEXT_PUBLIC_API_URL;
    const googleClientId = process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID;
    const handleGoogleLogin = async () => {
        console.log(apiUrl, googleClientId);
        const clientId = googleClientId ?? '';
        const redirectUri = encodeURIComponent(apiUrl ?? '');
        const scope = encodeURIComponent('profile email');
        const authUrl = `https://accounts.google.com/o/oauth2/v2/auth?client_id=${clientId}&redirect_uri=${redirectUri}&response_type=code&scope=${scope}&access_type=offline&prompt=consent`;
        window.location.href = authUrl;

    };

    return (
        <StyledButton
            onClick={handleGoogleLogin}
            icon={<GoogleIcon />}
            size="large"
            block
        >
            Continue with Google
        </StyledButton>
    );
};

export default GoogleLoginButton; 