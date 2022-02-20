using Microsoft.Extensions.Configuration;

namespace Gas.Common.Extensions;

public static class ConfigExtensions
{
    public static string GetDatabaseId(this IConfiguration config) => config.GetValue<string>("DatabaseName");
    public static string GetUsersContainerId(this IConfiguration config) => "users";
    public static string GetReportContainerId(this IConfiguration config) => config.GetValue<string>("ReportsContainer");
}