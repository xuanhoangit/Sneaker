using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.UI.Services;
using DotNetEnv;

using SneakerAPI.Core.Interfaces;
using SneakerAPI.Infrastructure.Data;
using SneakerAPI.Infrastructure.Repositories;
using System.Text;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces.UserInterfaces;
using SneakerAPI.Infrastructure.Repositories.UserRepositories;
using VNPAY.NET;

var  AllowHostSpecifiOrigins = "_allowHostSpecifiOrigins";
var builder = WebApplication.CreateBuilder(args);
Env.Load();
builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowHostSpecifiOrigins,
                      policy  =>
                      {
                            // policy.WithOrigins("http://127.0.0.1:5500")
                            policy.AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials(); // Only if using cookies/auth headers
                      });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<SneakerAPIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SneakerAPIConnection"),b=>b.MigrationsAssembly("SneakerAPI.AdminApi")));


builder.Services.AddScoped<IVnpay, Vnpay>();
builder.Services.AddTransient<IUnitOfWork,UnitOfWork>();
builder.Services.AddTransient<IEmailSender,EmailSender>();
builder.Services.AddTransient<IJwtService,JwtService>();
builder.Services.AddIdentity<IdentityAccount, IdentityRole<int>>()
    .AddEntityFrameworkStores<SneakerAPIDbContext>()
    .AddDefaultTokenProviders();

//*************** Tất cả config**
var config=builder.Configuration;
builder.Services.AddAuthentication()
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = config["Authentication:Google:ClientId"];
        options.ClientSecret = config["Authentication:Google:ClientSecret"];
        options.CallbackPath="/signin-google";
    });
//SetConfigEmailSettings
config["ConnectionStrings:SneakerAPIConnection"]=Environment.GetEnvironmentVariable("ConnectionString");
config["EmailSettings:SmtpServer"]=Environment.GetEnvironmentVariable("SmtpServer");
config["EmailSettings:SmtpPort"]=Environment.GetEnvironmentVariable("SmtpPort");
config["EmailSettings:SmtpUser"]=Environment.GetEnvironmentVariable("SmtpUser");
config["EmailSettings:SmtpPass"]=Environment.GetEnvironmentVariable("SmtpPass");
//SetConfigVNPAY
config["Vnpay:TmnCode"]=Environment.GetEnvironmentVariable("TmnCode");
config["Vnpay:HashSecret"]=Environment.GetEnvironmentVariable("HashSecret");
config["Vnpay:BaseUrl"]=Environment.GetEnvironmentVariable("BaseUrl");
config["Vnpay:ReturnUrl"]=Environment.GetEnvironmentVariable("ReturnUrl");
//SetDataEmailSettingModel
builder.Services.Configure<EmailSettings>(config.GetSection("EmailSettings"));

builder.Services.AddAuthentication()
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
          // Cấu hình RoleClaimType đúng với token của bạn
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
         ValidateIssuerSigningKey = true,
         ValidIssuer = Environment.GetEnvironmentVariable("JWT__ValidIssuer"),
         ValidAudience = Environment.GetEnvironmentVariable("JWT__ValidAudience"),
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT__Secret")))
    };
});
//End Cònig
// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache(); // Thêm dịch vụ MemoryCache
builder.Services.AddSession(); // Thêm dịch vụ Session
builder.Services.AddDistributedMemoryCache(); // Cần thiết cho Session

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();
app.UseCors(AllowHostSpecifiOrigins);
// Sử dụng Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 

app.Run();

