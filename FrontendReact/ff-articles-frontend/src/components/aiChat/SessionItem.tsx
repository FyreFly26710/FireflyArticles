import { useState } from 'react'
import { Button, Dropdown, Menu, Modal, message, Input } from 'antd'
import { MoreOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons'

interface SessionItemProps {
    session: API.SessionDto
    isActive: boolean
    onSelect: (session: API.SessionDto) => void
    onEdit: (session: API.SessionDto) => void
    onDelete: (session: API.SessionDto) => void
}

export default function SessionItem({
    session,
    isActive,
    onSelect,
    onEdit,
    onDelete
}: SessionItemProps) {
    const [isHovered, setIsHovered] = useState(false)
    const [isDeleteModalVisible, setIsDeleteModalVisible] = useState(false)
    const [isEditModalVisible, setIsEditModalVisible] = useState(false)
    const [editedName, setEditedName] = useState(session.sessionName)

    const handleDelete = () => {
        onDelete(session)
        setIsDeleteModalVisible(false)
        message.success('Session deleted successfully')
    }

    const handleEdit = () => {
        if (!editedName.trim()) {
            message.error('Session name cannot be empty')
            return
        }
        
        const updatedSession = { ...session, sessionName: editedName.trim() }
        onEdit(updatedSession)
        setIsEditModalVisible(false)
        message.success('Session updated successfully')
    }

    const openEditModal = () => {
        setEditedName(session.sessionName)
        setIsEditModalVisible(true)
    }

    const menu = (
        <Menu>
            <Menu.Item key="edit" icon={<EditOutlined />} onClick={openEditModal}>
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
                className={
                    "flex items-center p-3 cursor-pointer transition-colors rounded-lg mb-2"
                    + (isActive ? "bg-gray-100" : "")
                    + (isHovered ? "hover:bg-gray-100" : "")
                }
                onClick={() => onSelect(session)}
                onMouseEnter={() => setIsHovered(true)}
                onMouseLeave={() => setIsHovered(false)}
            >
                <div className="flex-1 min-w-0">
                    <div className="text-sm text-gray-800 mb-1 truncate">
                        {session.sessionName}
                    </div>
                    <div className="text-xs text-gray-500 flex items-center gap-2">
                        <span>{session.roundCount} chats</span>
                        <span>•</span>
                        <span>{new Date(session.updateTime).toLocaleDateString()}</span>
                    </div>
                </div>
                <div className="flex items-center gap-2">
                    {(isHovered || isActive ) && (session.roundCount > 0) && (
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

            <Modal
                title="Edit Session Name"
                open={isEditModalVisible}
                onOk={handleEdit}
                onCancel={() => setIsEditModalVisible(false)}
                okText="Save"
                cancelText="Cancel"
            >
                <div className="mt-4">
                    <Input
                        placeholder="Enter session name"
                        value={editedName}
                        onChange={(e) => setEditedName(e.target.value)}
                        onPressEnter={handleEdit}
                        autoFocus
                    />
                </div>
            </Modal>
        </>
    )
}
