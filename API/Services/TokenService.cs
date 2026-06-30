using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;
public class TokenService(IConfiguration configuration) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey =configuration["TokenKey"] ?? throw new Exception("Can not get key");
        if(tokenKey.Length<64)
        throw new Exception("your token key needs to be >= 64 characters");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims=new List<Claim>
        {
            new(ClaimTypes.Email,user.Email),
            new(ClaimTypes.NameIdentifier,user.Id)
        };

        var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor=new SecurityTokenDescriptor
        {
            Subject=new ClaimsIdentity(claims),
            Expires=DateTime.UtcNow.AddDays(7),
            SigningCredentials=creds

        };

        var tokenHandelar=new JwtSecurityTokenHandler();
        var token= tokenHandelar.CreateToken(tokenDescriptor);

        return tokenHandelar.WriteToken(token);
    }
}