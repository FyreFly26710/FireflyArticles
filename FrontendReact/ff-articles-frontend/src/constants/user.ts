// Default user
import ACCESS_ENUM from "@/access/accessEnum";

export const DEFAULT_USER: API.LoginUserResponse = {
    userName: "Guest",
    userProfile: "No description available",
    userAvatar: "https://gw.alipayobjects.com/zos/antfincdn/efFD%24IOql2/weixintupian_20170331104822.jpg",
    userRole: ACCESS_ENUM.NOT_LOGIN,
};