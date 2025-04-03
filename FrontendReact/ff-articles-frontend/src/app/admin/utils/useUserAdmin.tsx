import { useRef } from "react";
import { apiUserGetByPage } from "@/api/identity/api/user";

export const useUserAdmin = () => {
    const actionRef = useRef<any>();

    const fetchUsers = async (params: any, sort: any, filter: any) => {
        const sortField = Object.keys(sort)?.[0];
        const sortOrder = sort?.[sortField] ?? undefined;
        const { data, code } = await apiUserGetByPage({
            ...params,
            sortField,
            sortOrder,
            ...filter,
        } as API.apiUserGetByPageParams);
        return {
            success: code === 200,
            data: data?.data ?? [],
            total: Number(data?.counts) ?? 0,
        };
    };

    return {
        actionRef,
        fetchUsers,
    };
}; 