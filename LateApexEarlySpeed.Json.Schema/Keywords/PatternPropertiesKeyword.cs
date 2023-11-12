﻿using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using LateApexEarlySpeed.Json.Schema.Common;
using LateApexEarlySpeed.Json.Schema.Common.interfaces;
using LateApexEarlySpeed.Json.Schema.JSchema;
using LateApexEarlySpeed.Json.Schema.Keywords.JsonConverters;

namespace LateApexEarlySpeed.Json.Schema.Keywords;

[Keyword("patternProperties")]
[JsonConverter(typeof(PatternPropertiesKeywordJsonConverter))]
internal class PatternPropertiesKeyword : KeywordBase, ISchemaContainerElement
{
    private readonly Dictionary<string, (Regex regex, JsonSchema schema)> _patternSchemas;

    public PatternPropertiesKeyword(Dictionary<string, JsonSchema> patternSchemas)
    {
        _patternSchemas = patternSchemas.ToDictionary(
            kv => kv.Key, 
            kv => (new Regex(kv.Key, RegexOptions.Compiled, TimeSpan.FromMilliseconds(200)), kv.Value));
    }

    protected internal override ValidationResult ValidateCore(JsonElement instance, JsonSchemaOptions options)
    {
        if (instance.ValueKind != JsonValueKind.Object)
        {
            return ValidationResult.ValidResult;
        }

        foreach (JsonProperty jsonProperty in instance.EnumerateObject())
        {
            string propertyName = jsonProperty.Name;
            JsonElement propertyValue = jsonProperty.Value;

            foreach ((Regex regex, JsonSchema schema) patternSchema in _patternSchemas.Values)
            {
                if (patternSchema.regex.IsMatch(propertyName))
                {
                    ValidationResult validationResult = patternSchema.schema.Validate(propertyValue, options);
                    if (!validationResult.IsValid)
                    {
                        return validationResult;
                    }
                }
            }
        }

        return ValidationResult.ValidResult;
    }

    public ISchemaContainerElement? GetSubElement(string name)
    {
        return _patternSchemas.TryGetValue(name, out (Regex regex, JsonSchema schema) regexAndSchema) 
            ? regexAndSchema.schema 
            : null;
    }

    public IEnumerable<ISchemaContainerElement> EnumerateElements()
    {
        return _patternSchemas.Values.Select(regexAndSchema => regexAndSchema.schema);
    }

    public bool IsSchemaType => false;

    public JsonSchema GetSchema()
    {
        throw new InvalidOperationException();
    }

    public bool ContainsMatchedPattern(string propertyName)
    {
        return _patternSchemas.Values.Any(regexAndSchema => regexAndSchema.regex.IsMatch(propertyName));
    }
}