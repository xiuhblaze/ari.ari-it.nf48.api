using System;
using System.Text.Json;

namespace Arysoft.ARI.NF48.Api.Tools
{
    public static class JsonElementExtensions
    {
        public static Guid? GetNullableGuid(this JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var prop) || prop.ValueKind == JsonValueKind.Null)
                return null;
            return prop.GetGuid();
        }

        public static int? GetNullableInt(this JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var prop) || prop.ValueKind == JsonValueKind.Null)
                return null;
            return prop.GetInt32();
        }

        public static string GetNullableString(this JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var prop) || prop.ValueKind == JsonValueKind.Null)
                return null;
            return prop.GetString();
        }

        public static DateTime? GetNullableDateTime(this JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var prop) || prop.ValueKind == JsonValueKind.Null)
                return null;
            return prop.GetDateTime();
        }
    }
}