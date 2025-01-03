using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace Generators;

public class JsonSchemaToCSharp
{
    public async Task<string> GenerateAsync(string json)
    {
        var jsonContent = json ?? throw new NullReferenceException(nameof(json));
        try
        {
            var jsonText = jsonContent.ToString();
            var schema = await JsonSchema.FromJsonAsync(jsonText)
                ?? throw new InvalidDataException("Schema is null");
            CSharpGenerator generator = new(schema, new());
            return generator.GenerateFile();
        }
        catch (Exception ex)
        {
            throw ex.InnerException ?? ex; //Unwrap for ease of debugging.
        }
    }
}

[Generator(LanguageNames.CSharp)]
public class CustomGenerator : IIncrementalGenerator
{
    private static readonly JsonSchemaToCSharp _generator = new();
    record JsonTuple(string Name, string? Value);
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get all additional files with .json extension
        var jsonFiles = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase));
        var analyzerConfig = context.AnalyzerConfigOptionsProvider;
        // Register the source output
        context.RegisterSourceOutput(analyzerConfig, (spc, config) =>
        {
            var outputDir = config.GlobalOptions.TryGetValue("build_property.CompilerGeneratedFilesOutputPath", out var value)
                ? value
                : "/workspaces/mcp-dotnet-sdk/src/Abstractions/generated";//throw new InvalidDataException(string.Join("::", config.GlobalOptions.Keys));
            context.RegisterSourceOutput(jsonFiles, (spc, jsonFile) =>
            {
                var jsonContent = jsonFile?.GetText()?.ToString()
                    ?? throw new InvalidDataException("Json file content is null");
                var code = _generator.GenerateAsync(jsonContent).Result;
                var path = Path.GetFileNameWithoutExtension(jsonFile.Path);
                spc.AddSource($"{path}.types.g.cs", SourceText.From(code, Encoding.UTF8));
            });
        });
    }
}
