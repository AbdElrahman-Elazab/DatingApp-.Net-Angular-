using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;


public class AccountController(AppDbContext context, ITokenService tokenService) : BaseController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {

        if (await EmailExists(registerDto.email)) return BadRequest("Email Token");

        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            Email = registerDto.email,
            DisplayName = registerDto.displayName,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
            Passwordsalt = hmac.Key
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.ToDto(tokenService);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user is null) return Unauthorized("Invalid Credintials");

        using var hmac = new HMACSHA512(user.Passwordsalt);

        var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (int i = 0; i < ComputeHash.Length; i++)
        {
            if (ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("Invaild password");
        }

        return user.ToDto(tokenService);
    }
    private async Task<bool> EmailExists(string email)
    {
        return await context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }


}