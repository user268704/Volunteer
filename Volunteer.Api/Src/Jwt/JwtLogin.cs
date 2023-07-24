using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Volunteer.Api.Jwt;

public class JwtLogin : IJwtLogin
{
    private readonly IConfiguration _configuration;

    public JwtLogin(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetToken(List<Claim> claims)
    {
        var jwt = new JwtSecurityToken(
            claims: claims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}