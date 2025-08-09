using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Services;
using QRB.Services.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add Controllers for API
builder.Services.AddControllers();

// Configure session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "QRB.Session";
});

// Add Entity Framework
builder.Services.AddDbContext<QRBDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add custom services
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<AuthorizationSeeder>();
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

var app = builder.Build();

// Seed authorization data - Tạm thời disable để chạy thủ công script SQL
// using (var scope = app.Services.CreateScope())
// {
//     var seeder = scope.ServiceProvider.GetRequiredService<AuthorizationSeeder>();
//     await seeder.SeedAsync();
// }

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.UseStaticFiles();
app.MapRazorPages();
app.MapControllers(); // Add API controllers

app.Run();
