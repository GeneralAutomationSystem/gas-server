using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gas.Globals;
public static class Json
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

}