using System.IdentityModel.Tokens.Jwt;
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
        // TODO manca implementazione generazione token Sottosistema IoT (con validita' illimitata)
        throw new NotImplementedException();
    }
}