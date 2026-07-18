using System.Text;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.MiddleWare;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IPhotoService,PhotoService>();
builder.Services.AddScoped<IMemberRepository,MemberRepository>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var tokenKey=builder.Configuration["TokenKey"] 
    ?? throw new Exception("Can not find key -- Program.cs");
    options.TokenValidationParameters=new TokenValidationParameters
    {
        ValidateIssuerSigningKey=true,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        ValidateIssuer=false,
        ValidateAudience=false,
    };

});


    
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleWare>();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.UseCors(configure=>configure
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://localhost:4200","https://localhost:4200"));
app.MapControllers();


using var scope=app.Services.CreateScope();
var services=scope.ServiceProvider;
try
{
    var context=services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);
}
catch (System.Exception)
{
    var logger=services.GetRequiredService<ILogger<Program>>();
    logger.LogError("An error occured during migration");
    
    throw;
}
app.Run();
