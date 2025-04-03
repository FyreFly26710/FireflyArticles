/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './src/**/*.{js,ts,jsx,tsx,mdx}',
    ],
    theme: {
        extend: {
            colors: {
                primary: '#1890ff',
                secondary: '#52c41a',
                danger: '#ff4d4f',
                warning: '#faad14',
            },
        },
    },
    plugins: [],
    corePlugins: {
        preflight: true,
    },
} 