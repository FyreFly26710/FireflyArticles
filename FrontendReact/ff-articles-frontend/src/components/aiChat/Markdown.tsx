import ReactMarkdown from 'react-markdown'
import remarkGfm from 'remark-gfm'
import rehypeKatex from 'rehype-katex'
import remarkMath from 'remark-math'
import remarkBreaks from 'remark-breaks'
import { Prism as SyntaxHighlighter } from 'react-syntax-highlighter'
import { Button, message } from 'antd'
import { CopyOutlined } from '@ant-design/icons'
import { sanitizeUrl } from '@braintree/sanitize-url'
import { ReactNode } from 'react'

import 'katex/dist/katex.min.css'

interface MarkdownProps {
    children: string
    hiddenCodeCopyButton?: boolean
    className?: string
}

interface CodeBlockProps {
    children: ReactNode
    className?: string
    hiddenCodeCopyButton?: boolean
}

export default function Markdown({ children, hiddenCodeCopyButton, className }: MarkdownProps) {
    return (
        <ReactMarkdown
            remarkPlugins={[remarkGfm, remarkMath, remarkBreaks]}
            rehypePlugins={[rehypeKatex]}
            urlTransform={(url) => sanitizeUrl(url)}
            components={{
                code: ({ children, className, ...props }) => (
                    <CodeBlock
                        children={children}
                        className={className}
                        hiddenCodeCopyButton={hiddenCodeCopyButton}
                        {...props}
                    />
                ),
                a: ({ node, ...props }) => (
                    <a
                        {...props}
                        target="_blank"
                        rel="noreferrer"
                        onClick={(e) => e.stopPropagation()}
                    />
                ),
            }}
        >
            {children}
        </ReactMarkdown>
    )
}

function CodeBlock({ children, className, hiddenCodeCopyButton }: CodeBlockProps) {
    const match = /language-(\w+)/.exec(className || '')
    const language = match?.[1] || 'text'

    const handleCopy = async () => {
        try {
            await navigator.clipboard.writeText(String(children))
            message.success('Copied to clipboard')
        } catch (err) {
            message.error('Failed to copy')
        }
    }

    if (!String(children).includes('\n')) {
        return (
            <code className={`px-1 mx-1 rounded ${className || ''}`}>
                {children}
            </code>
        )
    }

    return (
        <div className="relative my-4">
            <div className="flex justify-between items-center px-2 py-1 bg-gray-100 rounded-t-md">
                <span className="text-sm text-gray-500">
                    {'<' + language.toUpperCase() + '>'}
                </span>
                {!hiddenCodeCopyButton && (
                    <Button
                        type="text"
                        icon={<CopyOutlined />}
                        onClick={handleCopy}
                        className="text-sm text-gray-500"
                    />
                )}
            </div>
            <SyntaxHighlighter
                language={language}
                //style={atomDark}
                customStyle={{
                    margin: 0,
                    borderTopLeftRadius: 0,
                    borderTopRightRadius: 0,
                    borderBottomLeftRadius: '4px',
                    borderBottomRightRadius: '4px',
                }}
            >
                {String(children).replace(/\n$/, '')}
            </SyntaxHighlighter>
        </div>
    )
}
