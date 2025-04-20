// 'use client'
// import { useEffect } from 'react'
// import { Typography, Button } from 'antd'
// import { EditOutlined } from '@ant-design/icons'
// import { cn } from '@/libs/utils'
// import { Session } from '@/types/chat'

// interface HeaderProps {
//     session: Session
//     onEditSession?: (session: Session) => void
//     autoGenerateTitle?: boolean
// }

// export default function Header({
//     session,
//     onEditSession,
//     autoGenerateTitle = true
// }: HeaderProps) {
//     useEffect(() => {
//         if (
//             autoGenerateTitle &&
//             session.name === 'Untitled' &&
//             session.messages.length >= 2
//         ) {
//             // You can implement title generation logic here
//             console.log('Generate title for session:', session.id)
//         }
//     }, [session.messages.length, autoGenerateTitle, session.id, session.name])

//     return (
//         <div className="border-b border-gray-200 pt-3 pb-2 px-4">
//             <div className="w-full mx-auto flex flex-row items-center justify-between">
//                 <div
//                     className="flex items-center cursor-pointer"
//                     onClick={() => onEditSession?.(session)}
//                 >
//                     <Typography.Title
//                         level={5}
//                         className="max-w-56 ml-3 truncate"
//                     >
//                         {session.name}
//                     </Typography.Title>
//                     <Button
//                         type="text"
//                         icon={<EditOutlined />}
//                         className="ml-2"
//                     />
//                 </div>
//                 {/* Toolbar component can be added here if needed */}
//             </div>
//         </div>
//     )
// }
