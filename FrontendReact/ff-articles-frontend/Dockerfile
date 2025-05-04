# docker build -t ffarticles-frontend . --no-cache
# docker run -d -p 3000:3000 --name ff-frontend ffarticles-frontend

FROM node:20-alpine AS builder

WORKDIR /app

# Copy package files
COPY package.json package-lock.json* ./

# Install dependencies
RUN npm ci && \
    npm cache clean --force

# Copy the entire project
COPY . .

# Set build arguments with defaults
ARG NEXT_PUBLIC_GOOGLE_CLIENT_ID=xxx
ARG NEXT_PUBLIC_BASE_URL=https://localhost:21000

# Set environment variables for the build
ENV NEXT_PUBLIC_GOOGLE_CLIENT_ID=$NEXT_PUBLIC_GOOGLE_CLIENT_ID
ENV NEXT_PUBLIC_BASE_URL=$NEXT_PUBLIC_BASE_URL
ENV NODE_ENV=production

# Build the application
RUN npm run build

# Use a smaller base image for the final stage
FROM node:20-alpine AS runner

WORKDIR /app

ENV NODE_ENV=production

# Create .next directory explicitly
RUN mkdir -p .next

# Copy .next directory separately
COPY --from=builder /app/.next ./.next
COPY --from=builder /app/public ./public
COPY --from=builder /app/package.json ./package.json
COPY --from=builder /app/package-lock.json* ./
COPY --from=builder /app/node_modules ./node_modules

# Set runtime environment variables
ARG NEXT_PUBLIC_GOOGLE_CLIENT_ID
ARG NEXT_PUBLIC_BASE_URL

ENV NEXT_PUBLIC_GOOGLE_CLIENT_ID=$NEXT_PUBLIC_GOOGLE_CLIENT_ID
ENV NEXT_PUBLIC_BASE_URL=$NEXT_PUBLIC_BASE_URL

EXPOSE 3000

# Run with minimal privileges
USER node

# Start Next.js
CMD ["node_modules/next/dist/bin/next", "start"]