using Microsoft.AspNetCore.Http;
using System.Text.Json;

public static class SessionExtensions
{
    public static void SetObject<T>(this ISession session, string key, T value)
    {
        session.Set(key, JsonSerializer.SerializeToUtf8Bytes(value));
    }

    public static T? GetObject<T>(this ISession session, string key)
    {
        if (session.TryGetValue(key, out var value))
        {
            return JsonSerializer.Deserialize<T>(value);
        }
        return default;
    }
}