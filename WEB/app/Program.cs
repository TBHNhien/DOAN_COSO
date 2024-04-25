using app.Dao;
using app.Data;
using app.Models.Momo;
using app.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model.Dao;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Thêm services vào container
builder.Services.AddScoped<UserDao>(); // Đăng ký UserDao
builder.Services.AddScoped<ContentDao>();
builder.Services.AddScoped<CategoryDao>();
builder.Services.AddScoped<ProductCategoryDao>();
builder.Services.AddScoped<ProductDao>();


builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));

builder.Services.AddScoped<IMomoService, MomoService>();


// Thêm services cho session vào container
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Ví dụ cài đặt timeout là 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString

    )
);

//builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

//identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
.AddDefaultTokenProviders()
.AddDefaultUI()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// Đăng ký DbContext với chuỗi kết nối đã lấy và cấu hình bổ sung
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString, sqlServerOptions =>
//        sqlServerOptions.EnableRetryOnFailure()
//    )
//);


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Đặt app.UseSession() trước app.UseStaticFiles() và app.UseRouting()
app.UseSession();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();

//app.MapAreaControllerRoute(
//name: "AdminArea",
//areaName: "Admin",
//pattern: "Admin/{controller=HomeAdmin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Định nghĩa các route
app.MapControllerRoute(
	name: "Shoes Detail",
	pattern: "chi-tiet/{MetaTitle}/{id}",
	defaults: new { controller = "Shoes", action = "Detail" }
);

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "admin",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
});



app.MapRazorPages();

app.Run();
