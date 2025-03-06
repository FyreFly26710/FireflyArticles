import {  GoogleOutlined } from '@ant-design/icons';
import "./index.css";
import React from 'react';

const GoogleLoginButton = () => {
    const apiUrl = process.env.NEXT_PUBLIC_API_URL;
    const googleClientId = process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID;
    const handleGoogleLogin = () => {
        const clientId = googleClientId??'';
        const redirectUri = encodeURIComponent(apiUrl??'');
        const scope = encodeURIComponent('profile email');
        const authUrl = `https://accounts.google.com/o/oauth2/v2/auth?client_id=${clientId}&redirect_uri=${redirectUri}&response_type=code&scope=${scope}&access_type=offline&prompt=consent`;
        window.location.href = authUrl;
    };

    return (
        <GoogleOutlined onClick={handleGoogleLogin} style={{ color: '#1877F2', fontSize:24 }} />

    );
};

export default GoogleLoginButton;
