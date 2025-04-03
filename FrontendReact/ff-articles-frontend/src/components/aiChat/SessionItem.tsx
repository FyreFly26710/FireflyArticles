import { useState } from 'react'
import { Button, Dropdown, Menu, Modal, message } from 'antd'
import { MoreOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons'
import { Session } from '@/types/chat'
import { cn } from '@/libs/utils'

interface SessionItemProps {
    session: Session
    isActive: boolean
    onSelect: (session: Session) => void
    onEdit: (session: Session) => void
    onDelete: (session: Session) => void
    className?: string
}

export default function SessionItem({
    session,
    isActive,
    onSelect,
    onEdit,
    onDelete,
    className
}: SessionItemProps) {
    const [isHovered, setIsHovered] = useState(false)
    const [isDeleteModalVisible, setIsDeleteModalVisible] = useState(false)

    const handleDelete = () => {
        onDelete(session)
        setIsDeleteModalVisible(false)
        message.success('Session deleted successfully')
    }

    const menu = (
        <Menu>
            <Menu.Item key="edit" icon={<EditOutlined />} onClick={() => onEdit(session)}>
                Edit
            </Menu.Item>
            <Menu.Item key="delete" icon={<DeleteOutlined />} onClick={() => setIsDeleteModalVisible(true)}>
                Delete
            </Menu.Item>
        </Menu>
    )

    return (
        <>
            <div
                className={cn(
                    "flex items-center p-3 cursor-pointer transition-colors rounded-lg mb-2",
                    isActive ? "bg-gray-100" : "bg-transparent",
                    "hover:bg-gray-100",
                    className
                )}
                onClick={() => onSelect(session)}
                onMouseEnter={() => setIsHovered(true)}
                onMouseLeave={() => setIsHovered(false)}
            >
                <div className="flex-1 min-w-0">
                    <div className="text-sm text-gray-800 mb-1 truncate">
                        {session.name}
                    </div>
                    <div className="text-xs text-gray-500 flex items-center gap-2">
                        <span>{session.messages.length} messages</span>
                        <span>•</span>
                        <span>{new Date().toLocaleDateString()}</span>
                    </div>
                </div>
                <div className="flex items-center gap-2">
                    {(isHovered || isActive) && (
                        <Dropdown overlay={menu} trigger={['click']}>
                            <Button
                                type="text"
                                icon={<MoreOutlined />}
                                onClick={(e) => e.stopPropagation()}
                            />
                        </Dropdown>
                    )}
                </div>
            </div>

            <Modal
                title="Delete Session"
                open={isDeleteModalVisible}
                onOk={handleDelete}
                onCancel={() => setIsDeleteModalVisible(false)}
                okText="Delete"
                cancelText="Cancel"
                okButtonProps={{ danger: true }}
            >
                <p>Are you sure you want to delete this session? This action cannot be undone.</p>
            </Modal>
        </>
    )
}
