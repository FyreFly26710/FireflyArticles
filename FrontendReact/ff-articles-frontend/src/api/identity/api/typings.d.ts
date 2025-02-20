declare namespace API {
  type apiUserDeleteByIdParams = {
    id: number;
  };

  type apiUserGetByIdParams = {
    id: number;
  };

  type apiUserGetByPageParams = {
    PageNumber?: number;
    PageSize?: number;
    SortField?: string;
    SortOrder?: string;
  };

  type BooleanApiResponse = {
    code?: number;
    message?: string;
    data?: boolean;
  };

  type Int32ApiResponse = {
    code?: number;
    message?: string;
    data?: number;
  };

  type LoginUserDto = {
    id?: number;
    userAccount?: string;
    userName?: string;
    userEmail?: string;
    userAvatar?: string;
    userProfile?: string;
    userRole?: string;
    createTime?: string;
  };

  type LoginUserDtoApiResponse = {
    code?: number;
    message?: string;
    data?: LoginUserDto;
  };

  type UserApiDto = {
    userId?: number;
    createTime?: string;
    userAccount?: string;
    userEmail?: string;
    userName?: string;
    userAvatar?: string;
    userProfile?: string;
    userRole?: string;
  };

  type UserApiDtoApiResponse = {
    code?: number;
    message?: string;
    data?: UserApiDto;
  };

  type UserDto = {
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

  type UserDtoPaged = {
    pageIndex?: number;
    pageSize?: number;
    counts?: number;
    data?: UserDto[];
  };

  type UserDtoPagedApiResponse = {
    code?: number;
    message?: string;
    data?: UserDtoPaged;
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

  type UserUpdateRequest = {
    id?: number;
    userEmail?: string;
    userName?: string;
    userAvatar?: string;
    userProfile?: string;
    userRole?: string;
  };
}
