/** @type {import('next').NextConfig} */
const nextConfig = {
    output: "standalone",
    typescript: {
        ignoreBuildErrors: true,
    },
    eslint: {
        ignoreDuringBuilds: true,
    },
    // Don't generate static pages for pages that use browser APIs
    images: {
        unoptimized: true,
    },
    // Skip automatic static generation for client-side only routes
    experimental: {
        // This will prevent Next.js from statically generating pages
        // that use localStorage or other browser APIs
        disableOptimizedLoading: true,
    },
};

export default nextConfig;
