using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Helper.VnPay;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.AreaViewLocationFormats.Insert(0, "/Areas/Shared/Views/{0}.cshtml");
        options.AreaViewLocationFormats.Insert(0, "/Areas/Shared/Views/{1}/{0}.cshtml");
        //// Để chắc chắn, bạn cũng có thể thêm vào danh sách tìm kiếm chung (phòng trường hợp dùng ngoài Area)
        options.ViewLocationFormats.Insert(0, "/Areas/Shared/Views/{0}.cshtml");
        options.ViewLocationFormats.Insert(0, "/Areas/Shared/Views/{1}/{0}.cshtml");
    });

builder.Services.AddScoped<SendOTP>();
builder.Services.AddScoped<NguoiDungRepository>();
builder.Services.AddScoped<DonHangRepository>();
builder.Services.AddScoped<SanPhamRepository>();
builder.Services.AddScoped<BinhLuanRepository>();
builder.Services.AddScoped<DanhGiaRepository>();
builder.Services.AddScoped<DonHangRepository>();
builder.Services.AddScoped<GioHangRepository>();
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
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/Login/SignInGoogle";
    options.SaveTokens = true; //Bật savetokens để lưu id_token
});

builder.Services.AddDbContext<WebBanThuocBvtvContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


app.Run();
