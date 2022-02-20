using System.Text.Json;

namespace Gas.Common.Static;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions DefaultSerialization = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}