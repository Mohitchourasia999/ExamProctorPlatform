using ExamProctorPlatform.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Unique ID generated every time the application starts
var serverInstanceId = Guid.NewGuid().ToString();
builder.Services.AddSingleton(serverInstanceId);

// Add services to the container.
builder.Services.AddControllersWithViews();

// MySQL Database configuration injection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Role-Based Cookies Authentication Security setup
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "UserSession";
        config.LoginPath = "/Account/Login";
        config.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// .NET 10 Static Assets handling
app.MapStaticAssets();

app.UseRouting();

app.UseAuthentication();

// Force logout after every server restart
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var currentServerId =
            context.RequestServices.GetRequiredService<string>();

        var loginServerId =
            context.User.FindFirst("ServerInstanceId")?.Value;

        if (loginServerId != currentServerId)
        {
            await context.SignOutAsync("CookieAuth");

            context.Response.Redirect("/Account/Login");
            return;
        }
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Automatically ensures local database and tables exist on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();