using System.Reflection;
using System.Text;
using AuthService.Configuration;
using AuthService.Data;
using AuthService.Messaging;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var cfg     = builder.Configuration;
var svc     = builder.Services;

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ──────────────── EF Core + Postgres ────────────
svc.AddDbContext<AuthContext>(opt =>
{
    var cs = cfg.GetConnectionString("DefaultConnection");
    opt.UseNpgsql(cs, o => o.MigrationsAssembly(typeof(AuthContext).Assembly.FullName));
});

/*──────────────────────── 2. Repositories ───────────────────*/
svc.AddScoped<IUserRepository        , UserRepository>();
svc.AddScoped<IRoleRepository        , RoleRepository>();
svc.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

/*──────────────────────── 3. Identity helpers ───────────────*/
svc.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

/*──────────────────────── 4. JWT service  ───────────────────*/
svc.Configure<JwtSettings>(cfg.GetSection("Jwt"));
svc.AddScoped<IJwtTokenService, JwtTokenService>();

/*──────────────────────── 5. Domain service ─────────────────*/
svc.AddScoped<IAuthService, AuthService.Services.AuthService>();

/*──────────────────────── 6. Outbox publisher ───────────────*/
svc.AddHostedService<OutboxPublisher>();

/*──────────────────────── 7. MassTransit ────────────────────*/
svc.AddMassTransit(x =>
{
    // (пока Consumers нет, но можно добавить позже)
    if (builder.Environment.IsDevelopment())
    {
        x.UsingInMemory((ctx, bus) => bus.ConfigureEndpoints(ctx));
    }
    else
    {
        x.UsingRabbitMq((ctx, mq) =>
        {
            mq.Host(cfg["Rabbit:Host"]!, "/", h =>
            {
                h.Username(cfg["Rabbit:User"] ?? "guest");
                h.Password(cfg["Rabbit:Pass"] ?? "guest");
            });
            mq.ConfigureEndpoints(ctx);
        });
    }
});

/*──────────────────────── 8. JWT-Authentication ─────────────*/
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
}
byte[] key = Encoding.UTF8.GetBytes(cfg["Jwt:Secret"]!);

svc.AddAuthentication("Bearer")
   .AddJwtBearer("Bearer", o =>
   {
       o.RequireHttpsMetadata = false;
       o.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer           = true,
           ValidateAudience         = true,
           ValidateLifetime         = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer   = cfg["Jwt:Issuer"],
           ValidAudience = cfg["Jwt:Audience"],
           IssuerSigningKey = new SymmetricSecurityKey(key)
       };
   });

svc.AddAuthorization();

/*──────────────────────── 9. Controllers + Swagger ──────────*/
svc.AddControllers();

svc.AddEndpointsApiExplorer();
svc.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth-Service", Version = "v1" });

    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        In           = ParameterLocation.Header,
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        Description  = "Введите токен в формате **Bearer &lt;token&gt;**"
    });

    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [ new OpenApiSecurityScheme
            { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }
        ] = Array.Empty<string>()
    });

    // чтобы XML-комменты попали в Swagger:
    var xml = Path.Combine(AppContext.BaseDirectory,
                           $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) o.IncludeXmlComments(xml);
});

var app = builder.Build();

/*──────────────────────── 10. HTTP Pipeline ─────────────────*/
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
