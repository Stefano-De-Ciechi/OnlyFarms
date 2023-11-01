namespace OnlyFarms.RestApi.Infrastructure;

public class Policy
{
    public const string IsAdmin = "IsAdmin";
    public const string IsFarmManager = "IsFarmManager";
    public const string IsWaterManager = "IsWaterManager";
    public const string IsIotSubsystem = "IsIotSubsystem";
    public const string IsAuthenticated = "IsAuthenticated";
}