using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;

namespace BuberDinner.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IDateTimeProvider _dateTimeProvider;
    public JwtTokenGenerator(IDateTimeProvider dateTimeProvider){
        _dateTimeProvider = dateTimeProvider;
    }
    public string GenerateToken(Guid userId, string firstName, string lastName)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("super-secret-key-123djiwbdiofahklfhasewihu@$542")
            ),SecurityAlgorithms.HmacSha256
        );
        var claims = new []
        {
            new Claim(JwtRegisteredClaimNames.Sub,userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName,firstName),
            new Claim(JwtRegisteredClaimNames.FamilyName,lastName),
        };

        var securityToken = new JwtSecurityToken(
            issuer:"BuberDinner",
            claims:claims,
            signingCredentials:signingCredentials,
            expires:_dateTimeProvider.UtcNow.AddDays(1)
            );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}