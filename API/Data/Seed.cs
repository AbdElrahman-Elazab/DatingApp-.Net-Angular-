using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Dtos;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(AppDbContext context)
    {
        if (await context.Members.AnyAsync()) return;
        var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

        if (members == null)
        {
            Console.WriteLine("No member in seed data");
            return;
        }
        foreach (var member in members)
        {
            var hmac = new HMACSHA512();

            var user = new AppUser
            {
                Id = member.Id,
                Email = member.Email,
                DisplayName = member.DisplayName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Password")),
                Passwordsalt = hmac.Key,
                ImageUrl = member.ImageUrl,
                Member = new Member
                {
                    Id = member.Id,
                    City = member.City,
                    Country = member.Country,
                    Created = member.Created,
                    DisplayName = member.DisplayName,
                    Gender = member.Gender,
                    DateOfBirth = member.DateOfBirth,
                    Description = member.Description,
                    ImageUrl = member.ImageUrl,
                    LastActive = member.LastActive,

                }
            };

            user.Member.Photoes.Add(new Photo
            {
                MemberId=member.Id,
                Url=member.ImageUrl
            });

        context.Users.Add(user);
        }
        await context.SaveChangesAsync();


    }

}