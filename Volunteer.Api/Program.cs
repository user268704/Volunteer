using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Volunteer.Api.Jwt;
using Volunteer.Api.Services;
using Volunteer.Api.Services.Events;
using Volunteer.Api.Services.Fillers;
using Volunteer.Api.Services.Users;
using Volunteer.Infrastructure;
using Volunteer.Mapping;
using Volunteer.Models.User;

var builder = WebApplication.CreateBuilder(args);

#region Services

// Add services to the container.
builder.Services.AddControllers();

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

#region Services

builder.Services.AddScoped<IJwtLogin, JwtLogin>();
builder.Services.AddScoped<IEventManager, EventManager>();
builder.Services.AddScoped<IUserManager, UserManager>();

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

    #region fill

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

    await next.Invoke();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();