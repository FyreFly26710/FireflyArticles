// @ts-ignore
/* eslint-disable */
import request from "@/libs/request";

/**GET /api/identity/users */
export async function apiUserGetByPage(
  params: API.apiUserGetByPageParams,
  options?: { [key: string]: any }
) {
  return request<API.UserDtoPagedApiResponse>("/api/identity/users", {
    method: "GET",
    params: {
      ...params,
    },
    ...(options || {}),
  });
}


// export async function apiUserUpdateByRequest(
//   body: API.UserUpdateRequest,
//   options?: { [key: string]: any }
// ) {
//   return request<API.BooleanApiResponse>("/api/identity/users", {
//     method: "POST",
//     headers: {
//       "Content-Type": "application/json",
//     },
//     data: body,
//     ...(options || {}),
//   });
// }

/**GET /api/identity/users/${param0} */
export async function apiUserGetById(
  params: API.apiUserGetByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.UserApiDtoApiResponse>(`/api/identity/users/${param0}`, {
    method: "GET",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/**DELETE /api/identity/users/${param0} */
export async function apiUserDeleteById(
  params: API.apiUserDeleteByIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.BooleanApiResponse>(`/api/identity/users/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}
