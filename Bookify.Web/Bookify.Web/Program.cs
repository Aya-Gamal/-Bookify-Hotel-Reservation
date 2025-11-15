using Bookify.Data.Data;
using Bookify.Data.Models;
using Bookify.Services.ModelsRepos;
using Bookify.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Connection String
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Identity Setup
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// ✅ Repositories
builder.Services.AddScoped<RoomRepo>();
builder.Services.AddScoped<RoomTypeRepo>();
builder.Services.AddScoped<BookingRepo>();

builder.Services.AddControllersWithViews();

// Add Session 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // مدة تخزين الـ cart في الجلسة
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// ✅ Configure Middleware
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
app.UseSession();

// ✅ Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    string[] roles = { "Admin", "Customer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var adminEmail = "admin@bookify.com";
    var adminPass = "Admin@123";
    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(admin, adminPass);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
            var userId = await userManager.GetUserIdAsync(admin);
            //user profile creation
            var profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId
            };
            context.UserProfiles.Add(profile);
            await context.SaveChangesAsync();
        }
    }
}

app.Run();
