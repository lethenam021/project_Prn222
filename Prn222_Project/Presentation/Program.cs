using BusinessLogic.Interface;
using BusinessLogic.Services;
using BusinessLogic.Validation;
using DataAccess.IRepo;
using DataAccess.Models;
using DataAccess.Repositories;
using Hangfire;
using Hangfire.SqlServer;
using Infrastructure.Configuration;
using Infrastructure.Interface;
using Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Nist;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost:6379";
    options.InstanceName = "MyApp_";
});

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<CloneEbayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions // Bảo Hangfire dùng SQL Server
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true // Tốt cho hiệu năng
    }));

builder.Services.AddHangfireServer();

builder.Services.AddHttpClient();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// Infrastructure Services
builder.Services.AddScoped<IEmailService, EmailService>();

// Business Logic Services


builder.Services.AddScoped<ISellerOrderService, SellerOrderService>();
builder.Services.AddScoped<IUserRepo, UserRepository>();
builder.Services.AddScoped<AuthValidator>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IStoreSevice, StoreService>();
builder.Services.AddScoped<IStoreRepo, StoreRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepo, ProductRepository>();

builder.Services.AddScoped<ICategoryRepo, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IInventoryRepo, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddScoped<ICouponRepo, CouponRepository>();
builder.Services.AddScoped<ICouponService, CouponService>();

builder.Services.AddScoped<IOrderRepo, OrderRepository>();
builder.Services.AddScoped<IShippingInfoRepo, ShippingInfoRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ✅ thay cho MapStaticAssets / WithStaticAssets

app.UseRouting();

app.UseAuthorization();
app.UseHangfireDashboard();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
