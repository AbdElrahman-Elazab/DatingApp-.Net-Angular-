using API.Dtos;
using API.Entities;
using API.Interfaces;
using Humanizer;

namespace API.Extentions;
public static class  AppUserExtention
{
    public static UserDto ToDto(this AppUser user,ITokenService tokenService)
    {
        return new UserDto
        {
            Id=user.Id,
            Email=user.Email,
            DisplayName=user.DisplayName,
            ImageUrl=user.ImageUrl,
            token=tokenService.CreateToken(user)
        };
    }
}