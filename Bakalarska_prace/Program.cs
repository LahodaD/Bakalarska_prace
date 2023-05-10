using Bakalarska_prace.Domain.Abstraction;
using Bakalarska_prace.Domain.Implementation;
using Bakalarska_prace.Models.Database;
using Bakalarska_prace.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Bakalarska_prace.Models.ApplicationServices.Abstract;
using Bakalarska_prace.Models.ApplicationServices.Implementation;

var builder = WebApplication.CreateBuilder(args);

//var sqlBuilder = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("MySqlConnectionString"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AutosalonDbContext>(optionBuilder =>
                                        optionBuilder.UseMySql(builder.Configuration.GetConnectionString("MySqlConnectionString"),
                                                                new MySqlServerVersion("8.0.26")));

builder.Services.AddScoped<FileUpload>(serviceProvider => new FileUpload(serviceProvider.GetRequiredService<IWebHostEnvironment>().WebRootPath));
builder.Services.AddScoped<IFileUpload>(serviceProvider => serviceProvider.GetRequiredService<FileUpload>());

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AutosalonDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);

    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.LoginPath = "/Security/Account/Login";
    options.LogoutPath = "/Security/Account/Logout";
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<ISecurityApplicationService, SecurityIdentityApplicationService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
