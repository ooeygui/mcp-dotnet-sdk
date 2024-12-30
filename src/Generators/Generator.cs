using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace Generators;

[Generator(LanguageNames.CSharp)]
public class CustomGenerator : IIncrementalGenerator
{
    record JsonTuple(string Name, string? Value);
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get all additional files with .json extension
        var jsonFiles = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

        // Register the source output
        context.RegisterSourceOutput(jsonFiles, (spc, jsonFile) =>
        {
            if(jsonFile == null) throw new ArgumentNullException(nameof(jsonFile));
            var jsonContent = jsonFile.GetText();
            if(jsonContent == null) throw new NullReferenceException(nameof(jsonContent));
            try
            {
                var jsonText = jsonContent.ToString();
                var schema = JsonSchema.FromJsonAsync(jsonText).Result
                    ?? throw new InvalidDataException("Schema is null");
                var generator = new CSharpGenerator(schema, new CSharpGeneratorSettings());
                var code = generator.GenerateFile();
                var path = Path.GetFileNameWithoutExtension(jsonFile.Path);
                spc.AddSource($"{path}.types.g.cs", SourceText.From(code, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        });
        
        context.RegisterPostInitializationOutput(static postInitializationContext =>
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            // postInitializationContext.AddSource("myGeneratedFile.g.cs",
            //     SourceText.From($@"
            //         namespace GeneratedNamespace;
                    
            //         public static partial class MyGeneratedClass
            //         {{
            //             //public static string MyGeneratedMethod() => ""Hello, World!"";
            //         }}", Encoding.UTF8));
        });
    }
}
