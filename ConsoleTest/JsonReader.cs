using System.Reflection;
using LateApexEarlySpeed.Json.Schema;
using LateApexEarlySpeed.Json.Schema.Keywords;

namespace Cwe.StudyConfigurationConverter;
public class JsonReader
{
    private static JsonValidator _validator;

    static JsonReader()
    {
        _validator = GetJsonValidator("schema.json");
    }

    internal object ParseJson(string json)
    {
        ValidateJson();
        return null;
    }

    private static JsonValidator GetJsonValidator(string schemaFile)
    {
        var schema = LoadSchema(schemaFile);
        var jsonValidator = new JsonValidator(schema);
        return jsonValidator;
    }

    public void ValidateJson()
    {
        var json = LoadEmbeddedResource("data.json");
        ValidateAgainstSchema(json, _validator);
    }

    private static string LoadSchema(string name) =>
        LoadEmbeddedResource(name);

    public static string LoadEmbeddedResource(string resourceName)
    {
        resourceName = resourceName.Replace("/", ".");
        var thisAssembly = Assembly.GetExecutingAssembly();
        var path = "ConsoleTest.Schema";
        var stream = thisAssembly.GetManifestResourceStream(path + "." + resourceName);

        if (stream == null)
        {
            throw new ArgumentException("Given name does not exist as embedded resource", nameof(resourceName));
        }

        var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    private void ValidateAgainstSchema(string json, JsonValidator validator)
    {
        var validationResult = validator.Validate(json);

        if (!validationResult.IsValid)
        {
            throw new Exception($"Schema validation failed. {validationResult.ErrorMessage} at {validationResult.InstanceLocation}");
        }
    }
}
