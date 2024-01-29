using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using auth_service.Data;
using auth_service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

var connString = builder.Configuration["ConnectionStrings:Default"];

builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(connString));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/auth",  async (AuthModel model , SignInManager<IdentityUser> signinManager)=>
{

    var signinResult = await signinManager.PasswordSignInAsync(model.UserName, model.Password, false, true);
    if (signinResult.Succeeded)
    {
        var jwt = GenerateJwt(model.UserName);
        return jwt;
    }
     

    string GenerateJwt(string userName)
    {
        var key = app.Configuration["EncryptionKey"] ?? "";
        var keyBytes = Encoding.ASCII.GetBytes(key);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    // return 403
    throw new Exception("Signin was not successful.");
});

app.Run();

