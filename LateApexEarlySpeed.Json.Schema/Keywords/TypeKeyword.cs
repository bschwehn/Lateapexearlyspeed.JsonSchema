﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using LateApexEarlySpeed.Json.Schema.Common;
using LateApexEarlySpeed.Json.Schema.Keywords.JsonConverters;

namespace LateApexEarlySpeed.Json.Schema.Keywords;

[Keyword("type")]
[JsonConverter(typeof(TypeKeywordJsonConverter))]
internal class TypeKeyword : KeywordBase
{
    private static readonly Dictionary<InstanceType, JsonValueKind> InstanceTypeJsonKindMap = new()
    {
        { InstanceType.Object, JsonValueKind.Object },
        { InstanceType.Array, JsonValueKind.Array },
        { InstanceType.Null, JsonValueKind.Null },
        { InstanceType.Number, JsonValueKind.Number },
        { InstanceType.String, JsonValueKind.String }
    };

    public InstanceType[] InstanceTypes { get; init; } = null!;

    protected internal override ValidationResult ValidateCore(JsonElement instance, JsonSchemaOptions options)
    {
        Debug.Assert(InstanceTypes.Length != 0);
        Unsafe.SkipInit(out ValidationResult validationResult);

        foreach (InstanceType instanceType in InstanceTypes)
        {
            validationResult = ValidateAgainstType(instance, instanceType, options);
            if (validationResult.IsValid)
            {
                return ValidationResult.ValidResult;
            }
        }

        return validationResult;
    }

    private ValidationResult ValidateAgainstType(JsonElement instance, InstanceType expectedInstanceType, JsonSchemaOptions options)
    {
        switch (expectedInstanceType)
        {
            case InstanceType.Integer:
                if (instance.ValueKind != JsonValueKind.Number)
                {
                    return ValidationResult.CreateFailedResult(ResultCode.InvalidTokenKind, GetErrorMessage(expectedInstanceType, instance.ValueKind), options.ValidationPathStack, Name);
                }

                string rawText = instance.GetRawText();
                int dotIdx = rawText.IndexOf('.');
                if (dotIdx != -1)
                {
                    ReadOnlySpan<char> fraction = rawText.AsSpan(dotIdx + 1);
                    foreach (char c in fraction)
                    {
                        if (c != '0')
                        {
                            return ValidationResult.CreateFailedResult(ResultCode.NotBeInteger, $"Expect type '{expectedInstanceType}' but actual is double-liked number", options.ValidationPathStack, Name);
                        }
                    }
                }

                break;
            case InstanceType.Boolean:
                if (instance.ValueKind != JsonValueKind.True && instance.ValueKind != JsonValueKind.False)
                {
                    return ValidationResult.CreateFailedResult(ResultCode.InvalidTokenKind, GetErrorMessage(expectedInstanceType, instance.ValueKind), options.ValidationPathStack, Name);
                }
                break;
            default:
                return ValidateJsonKind(instance, expectedInstanceType, options.ValidationPathStack);
        }

        return ValidationResult.ValidResult;
    }

    private ValidationResult ValidateJsonKind(JsonElement instance, InstanceType expectedInstanceType, ValidationPathStack validationPathStack)
    {
        return instance.ValueKind == InstanceTypeJsonKindMap[expectedInstanceType]
            ? ValidationResult.ValidResult 
            : ValidationResult.CreateFailedResult(ResultCode.InvalidTokenKind, GetErrorMessage(expectedInstanceType, instance.ValueKind), validationPathStack, Name);
    }

    private static string GetErrorMessage(InstanceType expectedType, JsonValueKind actualKind)
    {
        return $"Expect type '{expectedType}' but actual is '{actualKind}'";
    }
}

internal enum InstanceType
{
    Null,
    Boolean,
    Object,
    Array,
    Number,
    String,
    Integer
}