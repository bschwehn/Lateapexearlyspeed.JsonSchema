﻿using LateApexEarlySpeed.Json.Schema.Common;
using Xunit;

namespace LateApexEarlySpeed.Json.Schema.UnitTests;

public class Tests
{
    [Fact]
    public void RunForDoc()
    {
        string schema = """
        { 
          "type": "string",
          "format": "email"
        }
        """;
        var jsonValidator = new JsonValidator(schema);
        Assert.True(jsonValidator.Validate("\"hello@world.com\"").IsValid);

        ValidationResult result = jsonValidator.Validate("\"@world.com\"");
        Assert.False(result.IsValid);
        Assert.Equal("format", result.Keyword);
        Assert.Equal(ResultCode.InvalidFormat, result.ResultCode);
        Assert.Equal("Invalid string value for format:'email'", result.ErrorMessage);
    }
}