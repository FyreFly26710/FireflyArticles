declare namespace API {
  type BooleanApiResponse = {
    code?: number;
    data?: boolean;
    message?: string;
  };

  type DeleteByIdRequest = {
    id?: number;
  };

  type getAdminGetDtoIdParams = {
    id: number;
  };

  type Int32ApiResponse = {
    code?: number;
    data?: number;
    message?: string;
  };

  type LoginUserResponse = {
    id?: number;
    userAccount?: string;
    userName?: string;
    userEmail?: string;
    userAvatar?: string;
    userProfile?: string;
    userRole?: string;
    createTime?: string;
  };

  type LoginUserResponseApiResponse = {
    code?: number;
    data?: LoginUserResponse;
    message?: string;
  };

  type PageRequest = {
    pageNumber?: number;
    pageSize?: number;
    sortField?: string;
    sortOrder?: string;
  };

  type UserDto = {
    userId?: number;
    createTime?: string;
    userAccount?: string;
    userEmail?: string;
    userName?: string;
    userAvatar?: string;
    userProfile?: string;
    userRole?: string;
  };

  type UserDtoApiResponse = {
    code?: number;
    data?: UserDto;
    message?: string;
  };

  type UserLoginRequest = {
    userAccount?: string;
    userPassword?: string;
  };

  type UserRegisterRequest = {
    userAccount?: string;
    userPassword?: string;
    confirmPassword?: string;
  };

  type UserResponse = {
    id?: number;
    userAccount?: string;
    userName?: string;
    userEmail?: string;
    userAvatar?: string;
    userProfile?: string;
    userRole?: string;
    createTime?: string;
    updateTime?: string;
  };

  type UserResponsePageResponse = {
    pageIndex?: number;
    pageSize?: number;
    recordCount?: number;
    data?: UserResponse[];
  };

  type UserResponsePageResponseApiResponse = {
    code?: number;
    data?: UserResponsePageResponse;
    message?: string;
  };

  type UserUpdateRequest = {
    id?: number;
    userEmail?: string;
    userName?: string;
    userAvatar?: string;
    userProfile?: string;
    userRole?: string;
  };
}
