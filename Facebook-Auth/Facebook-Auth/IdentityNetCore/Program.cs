using System;
using IdentityNetCore.Data;
using IdentityNetCore.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapRazorPages();

var connString = app.Configuration["ConnectionStrings:Default"];

var facebookAppId=app.Configuration["FacebookAppId"] ?? "";
var facebookAppSecret=    app.Configuration["FacebookAppSecret"] ?? "";
var smtp = builder.Configuration.GetSection("Smtp");

ConfigureServices(builder.Services);

app.Run();

void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<ApplicationDBContext>(o => o.UseSqlServer(connString));
    services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>();
           
    services.Configure<IdentityOptions>(options => {

        options.Password.RequiredLength = 3;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = false;

        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

        options.SignIn.RequireConfirmedEmail = true;
            
    });

    services.ConfigureApplicationCookie(option=> {
        option.LoginPath = "/Identity/Signin";
        option.AccessDeniedPath = "/Identity/AccessDenied";
        option.ExpireTimeSpan = TimeSpan.FromHours(10);
    });

    services.AddAuthentication().AddFacebook(options=> {

        options.AppId = facebookAppId;
        options.AppSecret = facebookAppSecret;
    });

    services.Configure<SmtpOptions>(smtp);

    services.AddSingleton<IEmailSender, SmtpEmailSender>();

    services.AddControllersWithViews();
}
