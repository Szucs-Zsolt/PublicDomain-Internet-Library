using Azure.Identity;
using PublicDomainInternetLibrary.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IEmailSender, EmailSender>();

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
    name: "default",
// Módosítva! pattern: "{controller=Home}/{action=Index}/{id?}");
    pattern: "{controller=Book}/{action=Index}/{id?}");
app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roleNames = new[] { "Admin", "Librarian", "User" };
    foreach (var roleName in roleNames)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            Console.WriteLine("**********************************************");
            Console.WriteLine($"*** EXISTING ROLE: {roleName}");
            Console.WriteLine("**********************************************");
        }
        else
        {
            Console.WriteLine("**********************************************");
            Console.WriteLine($"*** MISSING ROLE: {roleName.ToString()}");
            Console.WriteLine("**********************************************");
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
    var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string email = "admin@admin";
    string password = "Jelszó1";
    if (await _userManager.FindByEmailAsync(email) == null)
    {
        Console.WriteLine("**********************************************");
        Console.WriteLine($"Nincs ilyen email-ű user: {email}");
        Console.WriteLine("**********************************************");
        //var admin = new IdentityUser()
        var admin = new PublicDomainInternetLibrary.Models.ApplicationUser()
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };
        await _userManager.CreateAsync(admin, password);
        await _userManager.AddToRoleAsync(admin, "Admin");
    }
    else
    {
        Console.WriteLine("**********************************************");
        Console.WriteLine($"Van ilyen email-� user: {email}");
        Console.WriteLine("**********************************************");
    }
}

app.Run();
