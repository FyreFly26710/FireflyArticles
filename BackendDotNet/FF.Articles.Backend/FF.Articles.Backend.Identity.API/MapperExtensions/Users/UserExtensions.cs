namespace FF.Articles.Backend.Identity.API.MapperExtensions.Users;
public static class UserExtensions
{
    public static UserDto ToDto(this User user)
    {
        var userDto = new UserDto();
        userDto.Id = user.Id;
        userDto.UserAccount = user.UserAccount;
        userDto.UserName = user.UserName;
        userDto.UserEmail = user.UserEmail;
        userDto.UserAvatar = user.UserAvatar;
        userDto.UserProfile = user.UserProfile;
        userDto.UserRole = user.UserRole;
        userDto.CreateTime = user.CreateTime;
        userDto.UpdateTime = user.UpdateTime;
        return userDto;
    }
    public static LoginUserDto ToLoginUserDto(this User user)
    {
        var loginUserDto = new LoginUserDto();
        loginUserDto.Id = user.Id;
        loginUserDto.UserAccount = user.UserAccount;
        loginUserDto.UserName = user.UserName;
        loginUserDto.UserEmail = user.UserEmail;
        loginUserDto.UserAvatar = user.UserAvatar;
        loginUserDto.UserProfile = user.UserProfile;
        loginUserDto.UserRole = user.UserRole;
        loginUserDto.CreateTime = user.CreateTime;
        return loginUserDto;
    }
    public static UserApiDto ToUserApiDto(this User user)
    {
        var userApiDto = new UserApiDto();
        userApiDto.UserId = user.Id;
        userApiDto.UserAccount = user.UserAccount;
        userApiDto.UserName = user.UserName;
        userApiDto.UserEmail = user.UserEmail;
        userApiDto.UserAvatar = user.UserAvatar;
        userApiDto.UserProfile = user.UserProfile;
        userApiDto.UserRole = user.UserRole;
        userApiDto.CreateTime = user.CreateTime;
        return userApiDto;
    }
}