using System.Text.Json;
using System.Text.Json.Nodes;
using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Application.Queries;

internal static class JobSearchQueriesHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static string NormalizeJsonFilter(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "{}";
        }

        try
        {
            var node = JsonNode.Parse(value);
            if (node is null)
            {
                return "{}";
            }

            RemoveNullProperties(node);
            return node.ToJsonString(JsonOptions);
        }
        catch (JsonException)
        {
            return value;
        }
    }

    private static void RemoveNullProperties(JsonNode node)
    {
        switch (node)
        {
            case JsonObject jsonObject:
            {
                var propertyNames = jsonObject
                    .Where(property => property.Value is null)
                    .Select(property => property.Key)
                    .ToArray();

                foreach (var propertyName in propertyNames)
                {
                    jsonObject.Remove(propertyName);
                }

                foreach (var property in jsonObject.ToList())
                {
                    if (property.Value is not null)
                    {
                        NormalizePropertyValue(jsonObject, property.Key, property.Value);
                        RemoveNullProperties(property.Value);
                    }
                }

                break;
            }
            case JsonArray jsonArray:
                foreach (var child in jsonArray)
                {
                    if (child is not null)
                    {
                        RemoveNullProperties(child);
                    }
                }

                break;
        }
    }

    private static void NormalizePropertyValue(JsonObject parent, string propertyName, JsonNode propertyValue)
    {
        if (!string.Equals(propertyName, "provider", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (propertyValue is not JsonValue jsonValue)
        {
            return;
        }

        if (!jsonValue.TryGetValue<int>(out var providerValue))
        {
            return;
        }

        if (!Enum.IsDefined(typeof(JobSearchProviderKind), providerValue))
        {
            return;
        }

        parent[propertyName] = Enum.GetName(typeof(JobSearchProviderKind), providerValue);
    }
}
