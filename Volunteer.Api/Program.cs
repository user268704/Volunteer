using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Volunteer.Api.Jwt;
using Volunteer.Api.Services.Events;
using Volunteer.Api.Services.Fillers;
using Volunteer.Api.Services.Notifications;
using Volunteer.Api.Services.Users;
using Volunteer.Api.Signal;
using Volunteer.Infrastructure;
using Volunteer.Mapping;
using Volunteer.Models.User;

var builder = WebApplication.CreateBuilder(args);

#region Services

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Profiler));
builder.Services.AddIdentity<UserIdentity, IdentityRole>(options =>
    {
        options.User.AllowedUserNameCharacters =
            "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<DataContext>()
    .AddRoles<IdentityRole>();

builder.Services.AddSwaggerGen(options =>
{
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IJwtLogin, JwtLogin>();
builder.Services.AddScoped<IEventManager, EventManager>();
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<INotificationManager, NotificationManager>();

#endregion

#region Users

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("volunteer", policyBuilder => policyBuilder.RequireRole("volunteer"));
    options.AddPolicy("verified_volunteer", policyBuilder => policyBuilder.RequireRole("verified_volunteer"));
    options.AddPolicy("admin", policyBuilder => policyBuilder.RequireRole("admin"));
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(config =>
    {
        config.LoginPath = "user/login";
        config.LogoutPath = "user/logout";

        config.AccessDeniedPath = "forbidden";
    });

#endregion

#region Data

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Volunteer.Api"));
});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Конфигурационный мидлвар
    app.Use(async (context, next) => 
{
    #region Roles

    var roleManager = context.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();

    if (await roleManager.RoleExistsAsync("admin") &&
        await roleManager.RoleExistsAsync("volunteer") &&
        await roleManager.RoleExistsAsync("verified_volunteer"))
    {
    }
    else
    {
        await roleManager.CreateAsync(new IdentityRole("admin"));
        await roleManager.CreateAsync(new IdentityRole("volunteer"));
        await roleManager.CreateAsync(new IdentityRole("verified_volunteer"));
    }

    #endregion

    #region Fill

    var dataContext = context.RequestServices.GetRequiredService<DataContext>();
    var configuration = context.RequestServices.GetRequiredService<IConfiguration>();

    List<IFillService> fillers = new()
    {
        new CityService(dataContext, configuration),
        new EventTypeService(dataContext, configuration)
    };

    foreach (IFillService filler in fillers)
    {
        filler.Fill();
    }

    #endregion

    #region SuperAdmin

    var userManager = context.RequestServices.GetRequiredService<UserManager<UserIdentity>>();

    UserIdentity? superAdmin = await userManager.FindByNameAsync("superadmin");

    if (superAdmin == null)
    {
        List<string> superAdminString =
            File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestData/superadmin.txt"))
                .Split('\n')
                .Select(x => x.Trim())
                .ToList();

        var result = await userManager.CreateAsync(new UserIdentity
        {
            Name = superAdminString[0],
            Lastname = superAdminString[0],
            Patronymic = superAdminString[0],
            UserName = superAdminString[0],
            Email = superAdminString[1]
        }, superAdminString[2]);

        if (result.Succeeded)
            userManager.AddToRolesAsync(superAdmin,
                new string[] { "admin", "verified_volunteer", "volunteer" });
        else if (!result.Succeeded) throw new Exception("Не удалось создать суперадмина");
    }

    #endregion

    await next.Invoke();
});
}

app.MapHub<SignalHub>("signal");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();