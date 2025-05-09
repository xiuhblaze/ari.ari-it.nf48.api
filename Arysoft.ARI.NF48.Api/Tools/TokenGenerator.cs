using Arysoft.ARI.NF48.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Text;

namespace Arysoft.ARI.NF48.Api.Tools
{
    internal static class TokenGenerator
    {
        public static string GenerateTokenJwt(User item)
        {
            var secretKey = ConfigurationManager.AppSettings["JwtKey"];
            var audienceToken = ConfigurationManager.AppSettings["JwtAudience"];
            var issuerToken = ConfigurationManager.AppSettings["JwtIssuer"];
            var expireTime = ConfigurationManager.AppSettings["JwtExpireMinutes"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, item.ID.ToString()),
                new Claim(ClaimTypes.Name, item.Username),
                new Claim(ClaimTypes.Email, item.Email)
            });

            if (item.Roles != null)
            {
                foreach (var role in item.Roles)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
                }
            }

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: credentials);

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);

            return jwtTokenString;
        } // GenerateTokenJwt
    }
}