namespace OnlyFarms.WebApp.Data;

public interface ITokenRepository
{
    public Token GenerateUserToken(int days);
    public Token GenerateIotSubsystemToken();
}