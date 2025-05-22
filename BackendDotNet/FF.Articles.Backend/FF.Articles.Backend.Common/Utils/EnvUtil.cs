namespace FF.Articles.Backend.Common.Utils;
public static class EnvUtil
{
    public static bool IsDevelopment() => !IsProduction();
    public static bool IsProduction() => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
}


