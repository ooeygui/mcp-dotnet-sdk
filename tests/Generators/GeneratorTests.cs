namespace Generators.Tests;
using Assert = NUnit.Framework.Assert;

[TestFixture, TestClass]
public class GeneratorTests
{
    [Test, TestMethod]
    public async Task Test1()
    {
        var generator = new JsonSchemaToCSharp();
        var json = File.ReadAllText("mcp.json");
        var code = await generator.GenerateAsync(json);
        code = code.Trim();
        Assert.That(code, Is.Not.Null.Or.Empty);
        throw new Exception(code);
    }
}