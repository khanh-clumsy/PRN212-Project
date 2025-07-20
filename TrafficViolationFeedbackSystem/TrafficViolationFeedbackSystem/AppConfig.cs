using Microsoft.Extensions.Configuration;
using System;

public static class AppConfig
{
    public static IConfigurationRoot Configuration { get; }

    static AppConfig()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
            .Build();
    }

    public static string GetConnectionString(string name = "DefaultConnection")
    {
        return Configuration.GetConnectionString(name);
    }
}
