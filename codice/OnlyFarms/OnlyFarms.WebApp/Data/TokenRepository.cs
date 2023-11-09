using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OnlyFarms.WebApp.Data;

public class TokenRepository : ITokenRepository
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Token GenerateUserToken(int days)
    {
        var user = _httpContextAccessor.HttpContext?.User ?? throw new NullReferenceException("Missing User");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: user.Claims,
            expires: DateTime.Now.AddDays(days),
            signingCredentials: credentials
        );

        return new Token()
        {
            Value = new JwtSecurityTokenHandler().WriteToken(token),
            ValidFrom = token.ValidFrom,
            ValidTo = token.ValidTo
        };
    }

    public Token GenerateIotSubsystemToken()
    {
        var user = _httpContextAccessor.HttpContext?.User ?? throw new NullReferenceException("Missing User");

        if (!user.HasClaim(nameof(Roles), Roles.FarmManager))
        {
            throw new UnauthorizedAccessException("you must be a FarmManager to generate a JWT token for an IoT subsystem");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claimsList = user.Claims.Where(claim => claim.Type != nameof(Roles));   // seleziona tutti i claim dell'utente meno Roles.FarmManager
        
        var token = new JwtSecurityToken(
            claims: claimsList.Append(new Claim(nameof(Roles), Roles.IoTSubSystem)),    // "rimpiazza" Roles.FarmManager con Roles.IotSubsystem
            expires: DateTime.Now.AddYears(1),      // validita' un anno
            signingCredentials: credentials
        );

        return new Token()
        {
            Value = new JwtSecurityTokenHandler().WriteToken(token),
            ValidFrom = token.ValidFrom,
            ValidTo = token.ValidTo
        };
    }
}