using Microsoft.EntityFrameworkCore;
using Examen.ApplicationCore.Interfaces;
using Examen.ApplicationCore.Services;
using Examen.Infrastructure.Data;
using Examen.Infrastructure.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Examen.Infrastructure;
using Examen.Web.Controllers;


var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddControllers();

builder.Services.AddDbContext<ExamenDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceMachine, ServiceMachine>();
builder.Services.AddScoped<IServiceTypeDefaut, ServiceTypeDefaut>();
builder.Services.AddScoped<IServiceProduit, ServiceProduit>();
builder.Services.AddScoped<IServiceResultatControle, ServiceResultatControle>();
builder.Services.AddScoped<IServiceUtilisateur, ServiceUtilisateur>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<INotificationService, ServiceNotification>();
// ── ✅ NOUVEAU : SignalR ───────────────────────────────────────────────────
builder.Services.AddSignalR();

// ── ✅ NOUVEAU : Services notifications ───────────────────────────────────
builder.Services.AddScoped<AlerteService>();

// ── ✅ NOUVEAU : Background Service (vérifie seuils toutes les X minutes) ─
builder.Services.AddScoped<IServiceAlerte, AlerteService>();

// ── JWT (inchangé) ────────────────────────────────────────────────────────
var key = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    // ✅ NOUVEAU : SignalR envoie le token via query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs/notifications"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
                                       Encoding.UTF8.GetBytes(key)),
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = "id"
    };
});

// ── CORS (inchangé + AllowCredentials déjà présent = OK pour SignalR) ────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();   // requis pour SignalR WebSocket
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ─────────────────────────────────────────────────────────────────────────
var app = builder.Build();

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

app.UseCors("AllowViteDev");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── ✅ NOUVEAU : Route SignalR Hub ────────────────────────────────────────
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();