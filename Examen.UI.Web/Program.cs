using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Examen.ApplicationCore.Services;
using Examen.Infrastructure;
using Examen.Infrastructure.Data;
using Examen.Infrastructure.Hubs;
using Examen.Infrastructure.Services;
using Examen.Web.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// JWT : éviter le mapping automatique des claims
// ============================================================

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// ============================================================
// CONTROLLERS + JSON
// ============================================================

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;

        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// ============================================================
// DATABASE
// ============================================================

builder.Services.AddDbContext<ExamenDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ============================================================
// UNIT OF WORK
// ============================================================

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ============================================================
// SERVICES MÉTIER
// ============================================================

builder.Services.AddScoped<IServiceMachine, ServiceMachine>();

builder.Services.AddScoped<IServiceTypeDefaut, ServiceTypeDefaut>();

builder.Services.AddScoped<IServiceProduit, ServiceProduit>();

builder.Services.AddScoped<
    IServiceResultatControle,
    ServiceResultatControle
>();

builder.Services.AddScoped<
    IServiceUtilisateur,
    ServiceUtilisateur
>();

builder.Services.AddHttpClient<
    IServicePredictionDefaut,
    ServicePredictionDefaut
>();

builder.Services.AddScoped<IServiceLot, ServiceLot>();

builder.Services.AddScoped<JwtService>();

builder.Services.AddScoped<ReportService>();

builder.Services.AddScoped<IServiceProfil, ServiceProfil>();

builder.Services.AddScoped<IServiceMenu, ServiceMenu>();

// ============================================================
// NOTIFICATIONS EMAIL
// ============================================================

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);

builder.Services.AddScoped<
    INotificationService,
    EmailNotificationService
>();

// ============================================================
// ALERTES
// ============================================================

builder.Services.AddScoped<
    IServiceAlerte,
    AlerteService
>();

builder.Services.AddHostedService<
    AlerteBackgroundService
>();

// ============================================================
// SIGNALR
// ============================================================

builder.Services.AddSignalR();

// ============================================================
// JWT AUTHENTICATION
// ============================================================

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "Jwt:Key est manquant dans appsettings.json."
    );
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException(
        "Jwt:Issuer est manquant dans appsettings.json."
    );
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "Jwt:Audience est manquant dans appsettings.json."
    );
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.SaveToken = true;

        // ====================================================
        // SIGNALR : récupération du token dans access_token
        // ====================================================

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken =
                    context.Request.Query["access_token"];

                var path =
                    context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments(
                        "/hubs/notifications"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };

        // ====================================================
        // VALIDATION DU TOKEN
        // ====================================================

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,

                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    ),

                // Notre claim personnalisé
                NameClaimType = "id",

                // IMPORTANT :
                // Nous utilisons "estAdmin"
                // et non ClaimTypes.Role
                RoleClaimType = "estAdmin",

                ClockSkew = TimeSpan.Zero
            };
    });

// ============================================================
// AUTHORIZATION
// ============================================================

builder.Services.AddAuthorization(options =>
{
    // --------------------------------------------------------
    // ADMIN
    // --------------------------------------------------------

    options.AddPolicy(
        "RequireAdmin",
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireClaim(
                "estAdmin",
                "true"
            );
        }
    );

    // --------------------------------------------------------
    // UTILISATEUR AUTHENTIFIÉ
    // --------------------------------------------------------

    options.AddPolicy(
        "RequireUser",
        policy =>
        {
            policy.RequireAuthenticatedUser();
        }
    );
});

// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowViteDev",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5173"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

// ============================================================
// SWAGGER
// ============================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// ============================================================
// BUILD
// ============================================================

var app = builder.Build();

// ============================================================
// DEVELOPMENT
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection();

    app.UseExceptionHandler("/error");
}

// ============================================================
// MIDDLEWARE
// ============================================================

app.UseCors("AllowViteDev");

app.UseStaticFiles();

// IMPORTANT : Authentication AVANT Authorization
app.UseAuthentication();

app.UseAuthorization();

// ============================================================
// CONTROLLERS
// ============================================================

app.MapControllers();

// ============================================================
// SIGNALR
// ============================================================

app.MapHub<NotificationHub>(
    "/hubs/notifications"
);

// ============================================================
// RUN
// ============================================================

app.Run();