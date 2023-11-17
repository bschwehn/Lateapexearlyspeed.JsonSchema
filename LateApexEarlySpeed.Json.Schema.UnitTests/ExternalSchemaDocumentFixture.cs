﻿using System.Text.Json;
using System.Text.Json.Nodes;

namespace LateApexEarlySpeed.Json.Schema.UnitTests;

public class ExternalSchemaDocumentsFixture
{
    public ExternalSchemaDocumentsFixture()
    {
        ExternalSchemaDocuments = PrepareRefRemoteDocuments();
    }

    public IEnumerable<string> ExternalSchemaDocuments { get; }

    private IEnumerable<string> PrepareRefRemoteDocuments()
    {
        string path = Path.Combine("JSON-Schema-Test-Suite", "remotes");

        List<JsonNode> schemaDocuments = ExtractAllSchemaDocumentsFrom(path, new Uri("http://localhost:1234"));

        return schemaDocuments.Select(doc => doc.ToJsonString());
    }

    private List<JsonNode> ExtractAllSchemaDocumentsFrom(string path, Uri uri)
    {
        var schemaDocs = new List<JsonNode>();
        var curDirectory = new DirectoryInfo(path);

        foreach (FileInfo fileInfo in curDirectory.EnumerateFiles())
        {
            var id = new Uri(uri, fileInfo.Name);
            using (FileStream fs = fileInfo.OpenRead())
            {
                JsonNode jsonNode = JsonNode.Parse(fs)!;
                jsonNode.AsObject()["$id"] = JsonSerializer.SerializeToNode(id);
                schemaDocs.Add(jsonNode);
            }
        }

        foreach (DirectoryInfo subDirectory in curDirectory.EnumerateDirectories())
        {
            schemaDocs.AddRange(ExtractAllSchemaDocumentsFrom(Path.Combine(path, subDirectory.Name), new Uri(uri, subDirectory.Name + "/")));
        }

        return schemaDocs;
    }
}