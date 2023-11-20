﻿using System.Text.Json;
using LateApexEarlySpeed.Json.Schema.Common;

namespace LateApexEarlySpeed.Json.Schema.JInstance;

public readonly struct JsonInstanceElement : IEquatable<JsonInstanceElement>
{
    private readonly JsonElement _jsonElement;
    private readonly ImmutableJsonPointer _instanceLocation;

    public JsonInstanceElement(JsonElement jsonElement, ImmutableJsonPointer instanceLocation)
    {
        _jsonElement = jsonElement;
        _instanceLocation = instanceLocation;
    }

    public IEnumerable<JsonInstanceProperty> EnumerateObject()
    {
        JsonElement.ObjectEnumerator objectEnumerator = _jsonElement.EnumerateObject();
        foreach (JsonProperty jsonProperty in objectEnumerator)
        {
            yield return new JsonInstanceProperty(jsonProperty, _instanceLocation.Add(jsonProperty.Name));
        }
    }

    public IEnumerable<JsonInstanceElement> EnumerateArray()
    {
        int idx = 0;

        foreach (JsonElement item in _jsonElement.EnumerateArray())
        {
            yield return new JsonInstanceElement(item, _instanceLocation.Add(idx++));
        }
    }

    public ImmutableJsonPointer Location => _instanceLocation;

    public JsonValueKind ValueKind => _jsonElement.ValueKind;

    public string? GetString() => _jsonElement.GetString();

    public double GetDouble() => _jsonElement.GetDouble();

    public string GetRawText()
    {
        return _jsonElement.GetRawText();
    }

    public bool Equals(JsonInstanceElement other)
    {
        if (ValueKind != other.ValueKind)
        {
            return false;
        }

        switch (ValueKind)
        {
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                return true;

            case JsonValueKind.String:
                return GetString() == other.GetString();

            case JsonValueKind.Number:
                return GetRawText() == other.GetRawText();

            case JsonValueKind.Array:
                return EnumerateArray().SequenceEqual(other.EnumerateArray());

            case JsonValueKind.Object:
                Dictionary<string, JsonInstanceElement> otherProperties = other.EnumerateObject().ToDictionary(prop => prop.Name, prop => prop.Value);
                return EnumerateObject().Count() == otherProperties.Count
                       && EnumerateObject().All(prop => otherProperties.TryGetValue(prop.Name, out JsonInstanceElement otherValue) && otherValue.Equals(prop.Value));
            default:
                return false;
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is JsonInstanceElement other && Equals(other);
    }

    public static bool operator ==(JsonInstanceElement left, JsonInstanceElement right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(JsonInstanceElement left, JsonInstanceElement right)
    {
        return !left.Equals(right);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public static JsonInstanceElement ParseValue(ref Utf8JsonReader reader)
    {
        return new JsonInstanceElement(JsonElement.ParseValue(ref reader), ImmutableJsonPointer.Empty);
    }
}