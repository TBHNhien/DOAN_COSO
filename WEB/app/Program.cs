using app.Dao;
using app.Data;
using app.Models.Momo;
using app.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
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

// Đăng ký các dịch vụ xác thực
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.Cookie.HttpOnly = true;
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Bắt buộc HTTPS
		options.Cookie.SameSite = SameSiteMode.None; // Cho phép chia sẻ cookie
		options.LoginPath = "Identity/Account/Login";

		options.Events.OnRedirectToLogin = context =>
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return Task.CompletedTask;
		};
	});

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//	.AddCookie(options =>
//	{
//		options.Cookie.HttpOnly = true;
//		options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Bắt buộc HTTPS
//		options.Cookie.SameSite = SameSiteMode.None; // Cho phép chia sẻ cookie
//		options.LoginPath = "/Account/Login";
//		options.AccessDeniedPath = "/Account/AccessDenied";
//	});


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




//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("AllowSpecificOrigin",
//		builder => builder.AllowAnyHeader()
//						  .AllowAnyMethod()
//						  .AllowCredentials()
//						  .WithOrigins("http://localhost:5184")); // Điều chỉnh miền theo nhu cầu
//});

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin",
		builder => builder
			.WithOrigins("http://localhost:7053") // Thay bằng domain tương ứng
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials());
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");


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

app.UseAuthentication();  // Thêm middleware xác thực

app.UseAuthorization();   // Đảm bảo có cả middleware ủy quyền

app.MapRazorPages();


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
	endpoints.MapControllers(); // Map cho tất cả các API
	endpoints.MapControllerRoute(
        name: "admin",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
});





app.MapRazorPages();

app.Run();
