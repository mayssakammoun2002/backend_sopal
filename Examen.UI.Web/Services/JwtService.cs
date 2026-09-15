using Examen.ApplicationCore.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Examen.ApplicationCore.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(Utilisateur user)
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"];
            var jwtAudience = _config["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new InvalidOperationException("Jwt:Key est manquant dans appsettings.json.");

            if (string.IsNullOrWhiteSpace(jwtIssuer))
                throw new InvalidOperationException("Jwt:Issuer est manquant dans appsettings.json.");

            if (string.IsNullOrWhiteSpace(jwtAudience))
                throw new InvalidOperationException("Jwt:Audience est manquant dans appsettings.json.");

            bool isAdmin = user.Profil?.EstAdmin == true;

            var claims = new List<Claim>
            {
                // ID utilisateur
                new Claim("id", user.Id.ToString()),

                // Email
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),

                // Profil
                new Claim(
                    "profilId",
                    user.ProfilId?.ToString() ?? string.Empty
                ),

                // Administrateur ou non
                new Claim(
                    "estAdmin",
                    isAdmin ? "true" : "false"
                )
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}