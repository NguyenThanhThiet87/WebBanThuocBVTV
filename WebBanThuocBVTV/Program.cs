using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Helper.VnPay;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing required configuration: ConnectionStrings:DefaultConnection.");

var requiredConfigurationKeys = new[]
{
    "Authentication:Google:ClientId",
    "Authentication:Google:ClientSecret",
    "Brevo:ApiKey",
    "Brevo:SenderEmail",
    "Brevo:SenderName",
    "SmsSettings:AccountSID",
    "SmsSettings:AuthToken",
    "SmsSettings:OwnPhone",
    "SmsSettings:VerifyServiceSID",
    "OtpSettings:SecretKey",
    "Server:Url",
    "Vnpay:TmnCode",
    "Vnpay:HashSecret",
    "Vnpay:BaseUrl",
    "Vnpay:PaymentBackReturnUrl",
    "Cloudinary:cloud_name",
    "Cloudinary:api_key",
    "Cloudinary:api_secret"
};

var missingConfigurationKeys = requiredConfigurationKeys
    .Where(key => string.IsNullOrWhiteSpace(builder.Configuration[key]))
    .ToArray();

if (missingConfigurationKeys.Length > 0)
{
    throw new InvalidOperationException($"Missing required configuration: {string.Join(", ", missingConfigurationKeys)}.");
}

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.AreaViewLocationFormats.Insert(0, "/Areas/Shared/Views/{0}.cshtml");
        options.AreaViewLocationFormats.Insert(0, "/Areas/Shared/Views/{1}/{0}.cshtml");
        //// 
        options.ViewLocationFormats.Insert(0, "/Areas/Shared/Views/{0}.cshtml");
        options.ViewLocationFormats.Insert(0, "/Areas/Shared/Views/{1}/{0}.cshtml");
    });

builder.Services.AddHttpClient();
builder.Services.AddScoped<SendOTP>();
builder.Services.AddScoped<NguoiDungRepository>();
builder.Services.AddScoped<DonHangRepository>();
builder.Services.AddScoped<SanPhamRepository>();
builder.Services.AddScoped<BinhLuanRepository>();
builder.Services.AddScoped<DanhGiaRepository>();
builder.Services.AddScoped<DonHangRepository>();
builder.Services.AddScoped<GioHangRepository>();
builder.Services.AddScoped<HinhAnhRepository>();
builder.Services.AddScoped<NhomSanPhamRepository>();
builder.Services.AddScoped<NhaSanXuatRepository>();
builder.Services.AddScoped<TrangThaiRepository>();
builder.Services.AddScoped<IVnPayService, VnPayService>();


//Add services session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Thêm authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.SaveTokens = true;
    options.Scope.Add("email");
    options.Scope.Add("profile");
    options.Events = new OAuthEvents
    {
        OnRedirectToAuthorizationEndpoint = ctx =>
        {
            var sep = ctx.RedirectUri.Contains("?") ? "&" : "?";
            ctx.Response.Redirect(ctx.RedirectUri + sep + "prompt=select_account");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddDbContext<WebBanThuocBvtvContext>(options =>
    options.UseSqlServer(connectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


app.Run();
