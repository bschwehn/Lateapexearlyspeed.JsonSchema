﻿using System.Reflection;
using System.Text.Json.Serialization;
using LateApexEarlySpeed.Json.Schema.Keywords.JsonConverters;

namespace LateApexEarlySpeed.Json.Schema.Keywords;

[Obfuscation(ApplyToMembers = false)]
[Keyword("maxLength")]
[JsonConverter(typeof(BenchmarkValueKeywordJsonConverter<MaxLengthKeyword>))]
internal class MaxLengthKeyword : StringLengthKeywordBase
{
    protected override bool IsStringLengthInRange(int instanceStringLength)
    {
        return instanceStringLength <= BenchmarkValue;
    }

    protected override string GetErrorMessage(int instanceStringLength)
    {
        return ErrorMessage(instanceStringLength, BenchmarkValue);
    }

    [Obfuscation]
    public static string ErrorMessage(int instanceLength, uint maxLength)
    {
        return $"String instance's length is {instanceLength} which is greater than '{maxLength}'";
    }
}