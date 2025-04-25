using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.UI.Services;
using DotNetEnv;


using SneakerAPI.AdminApi.Controllers.ProductControllers;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Infrastructure.Data;
using SneakerAPI.Infrastructure.Repositories;
using System.Text;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces.UserInterfaces;
using SneakerAPI.Infrastructure.Repositories.UserRepositories;
using VNPAY.NET;
using Microsoft.AspNetCore.Authentication.JwtBearer;



var  AllowHostSpecifiOrigins = "_allowHostSpecifiOrigins";
var builder = WebApplication.CreateBuilder(args);
Env.Load();

builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
// var config=builder.Configuration;
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowHostSpecifiOrigins,
                      policy =>
                      {
                          policy
                            .WithOrigins(Environment.GetEnvironmentVariable("OriginHost"),Environment.GetEnvironmentVariable("OrginHost1"))
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                      });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<SneakerAPIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SneakerAPIConnection"),b=>b.MigrationsAssembly("SneakerAPI.AdminApi")));

builder.WebHost.ConfigureKestrel(serverOptions =>
{   
    serverOptions.ListenAnyIP(5000); // HTTP
    serverOptions.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.UseHttps(Environment.GetEnvironmentVariable("FILECERT"), "mypassword");
    });
});

builder.Services.AddScoped<IVnpay,Vnpay>();
builder.Services.AddTransient<IUnitOfWork,UnitOfWork>();
builder.Services.AddTransient<IEmailSender,EmailSender>();
builder.Services.AddTransient<IJwtService,JwtService>();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IGHN,GHN>();
builder.Services.AddIdentity<IdentityAccount, IdentityRole<int>>()
    .AddEntityFrameworkStores<SneakerAPIDbContext>()
    .AddDefaultTokenProviders();
//*************** Tất cả config**
var config=builder.Configuration;
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
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// Cấu hình Authentication với JWT
builder.Services.AddAuthentication(
    options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
)  
.AddJwtBearer(options =>
{   
    options.TokenValidationParameters = new TokenValidationParameters
    {
         ValidateIssuer = true,
          // Cấu hình RoleClaimType đúng với token của bạn
         RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = Environment.GetEnvironmentVariable("JWT__ValidIssuer"),
         ValidAudience = Environment.GetEnvironmentVariable("JWT__ValidAudience"),
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT__Secret")))
    };
});

//End Cònig
// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddMemoryCache(); // Thêm dịch vụ MemoryCache
builder.Services.AddSession(); // Thêm dịch vụ Session
builder.Services.AddDistributedMemoryCache(); // Cần thiết cho Session

var app = builder.Build();
// Thực hiện seed Role và tài khoản Admin

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var uow = services.GetRequiredService<IUnitOfWork>();
        await SeedRoleAdmin.InitializeAccount(services, uow);
        await SeedRoleAdmin.InitBrand(uow);
        SeedRoleAdmin.InitColor(uow);
        SeedRoleAdmin.InitCategory(uow);
        SeedRoleAdmin.InitProductCategory(uow);
        SeedRoleAdmin.InitSize(uow);
        SeedRoleAdmin.InitProductColor(uow);
        SeedRoleAdmin.InitProductColorSize(uow);
        // SeedRoleAdmin.UpdatePrice(uow);
        // SeedRoleAdmin.UpdateDateProduct(uow);
        // SeedRoleAdmin.UpdatePhoneNumber(uow);
        // for (int i = 0; i < 20; i++)
        // {

        // SeedRoleAdmin.InitCart(uow);
        // SeedRoleAdmin.InitOrder(uow);
        // }
    }
    catch (Exception ex)
    {
        // Log lỗi nếu cần
        Console.WriteLine("hahaha" + ex.Message);
    }
}

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

