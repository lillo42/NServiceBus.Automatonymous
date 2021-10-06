using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Automatonymous;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyModel;
using NServiceBus.Automatonymous.SourceGeneration;

namespace NServiceBus.Automatonymous.Tests.Generators;

public abstract class BaseTest
{
    protected virtual async Task<GeneratorDriver> GenerateMapperAsync(string code)
    {
        var node = CSharpSyntaxTree.ParseText(code);
        var syntaxTrees = new List<SyntaxTree>();
        foreach (var file in GetFiles("../../../../../src/NServiceBus.Automatonymous"))
        {
            syntaxTrees.Add(CSharpSyntaxTree.ParseText(await File.ReadAllTextAsync(file)));
        }

        var compilation = CSharpCompilation.Create("NServiceBus.Automatonymous.Test.Generated")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(DependencyContext.Default
                .CompileLibraries
                .SelectMany(cl => cl.ResolveReferencePaths())
                .Select(asm => MetadataReference.CreateFromFile(asm)))
            .AddReferences(MetadataReference.CreateFromFile(typeof(StateMachine).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(Saga).Assembly.Location))
            .AddSyntaxTrees(syntaxTrees)
            .AddSyntaxTrees(node);
            
        var driver = CSharpGeneratorDriver.Create(new NServiceBusSagaSourceGenerator());
        return driver.RunGenerators(compilation);
    }

    private static IEnumerable<string> GetFiles(string path)
    {
        foreach (var file in Directory.GetFiles(path, "*.cs"))
        {
            yield return file;
        }

        foreach (var directory in Directory.GetDirectories(path))
        {
            if (directory.EndsWith("obj") || directory.EndsWith("bin"))
            {
                continue;
            }
                
            foreach (var file in GetFiles(directory))
            {
                yield return file;
            }
        }
    }
}