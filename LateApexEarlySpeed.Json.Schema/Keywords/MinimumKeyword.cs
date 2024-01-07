﻿using System.Reflection;
using System.Text.Json.Serialization;
using LateApexEarlySpeed.Json.Schema.Keywords.JsonConverters;

namespace LateApexEarlySpeed.Json.Schema.Keywords;

[Obfuscation(ApplyToMembers = false)]
[Keyword("minimum")]
[JsonConverter(typeof(NumberRangeKeywordJsonConverter<MinimumKeyword>))]
internal class MinimumKeyword : NumberRangeKeywordBase
{
    protected override bool IsInRange(double instanceValue) 
        => instanceValue >= BenchmarkValue;

    protected override string GetErrorMessage(double instanceValue)
        => ErrorMessage(instanceValue, BenchmarkValue);

    [Obfuscation]
    public static string ErrorMessage(double instanceValue, double minimum)
    {
        return $"Instance '{instanceValue}' is less than '{minimum}'";
    }
}