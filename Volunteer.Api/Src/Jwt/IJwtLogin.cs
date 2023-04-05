using System.Security.Claims;

namespace Volunteer.Api.Jwt;

public interface IJwtLogin
{
    string GetToken(List<Claim> claims);
}