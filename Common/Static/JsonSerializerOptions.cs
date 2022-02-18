using System.Reflection.Metadata;
using System.Text.Json;

namespace Gas.Common.Static;

public static class JsonSerializerOptions
{
    public static readonly System.Text.Json.JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}