using System.IO;
using System.Text.Json;
using AutoScrew.Hmi.Models;

namespace AutoScrew.Hmi.Services;

public static class TemplateJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public static void Save(string path, TemplateDocument document)
    {
        var json = JsonSerializer.Serialize(document, Options);
        File.WriteAllText(path, json);
    }

    public static TemplateDocument Load(string path)
    {
        var json = File.ReadAllText(path);
        var doc = JsonSerializer.Deserialize<TemplateDocument>(json, Options)
                  ?? throw new InvalidDataException("Empty or invalid template JSON.");
        return doc;
    }
}
