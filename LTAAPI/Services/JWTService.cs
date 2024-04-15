using LTAAPI.Interfaces;
using LTAAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LTAAPI.Services
{
    public class JWTService : IJWTRepository
    {
        public readonly IConfiguration configuration;
        public JWTService(IConfiguration config)
        {
            this.configuration = config;
        }
        public string GenerateJWTToken(UsersModel model)
        {
            try
            {
                var seccurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:key"]!));
                var credentials = new SigningCredentials(seccurityKey, SecurityAlgorithms.HmacSha256Signature);


                var userclaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, model.ID.ToString()),
                    new Claim(ClaimTypes.Name, model.UserName.ToString())
                };

                var jwttoken = new JwtSecurityToken(
                    claims: userclaims,
                    issuer: configuration["JWT:Issuer"],
                    audience: configuration["JWT:Audience"],
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddDays(30),
                    signingCredentials: credentials

                    );

                return new JwtSecurityTokenHandler().WriteToken(jwttoken);
            }
            catch (Exception ex)
            {


            }

            return String.Empty;
        }
    }
}
