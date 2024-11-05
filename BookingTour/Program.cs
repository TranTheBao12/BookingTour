using BookingTour.Areas.Admin.Controllers;
using BookingTour.Data;
using BookingTour.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// C?u hình k?t n?i c? s? d? li?u
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<YourExistingDbContextName>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// C?u hình Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// C?u hình xác th?c v?i Google và Facebook
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
        options.CallbackPath = "/signin-facebook";
    });

// C?u hình SmsService
builder.Services.AddTransient<SmsService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var accountSid = configuration["Twilio:AccountSid"];
    var authToken = configuration["Twilio:AuthToken"];
    var twilioPhoneNumber = configuration["Twilio:PhoneNumber"];
    return new SmsService(accountSid, authToken, twilioPhoneNumber);
});

// C?u hình Razor Pages và Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// C?u hình Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Home/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{

    endpoints.MapAreaControllerRoute(
        name: "ADMIN",
        areaName: "Admin",
        pattern: "Admin/{controller=Home}/{action=AccessDenied}/{id?}"); // ??nh ngh?a route cho area ADMIN

    endpoints.MapAreaControllerRoute(
    name: "HOST",
    areaName: "Host",
    pattern: "Host/{controller=Home}/{action=AccessDenied}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Tours}/{action=Index}/{id?}");


});

app.MapRazorPages();

app.Run();
