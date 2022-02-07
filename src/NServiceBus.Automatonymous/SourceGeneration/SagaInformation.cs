using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NServiceBus.Automatonymous.SourceGeneration;

internal class SagaInformation
{
    public SagaInformation(ClassDeclarationSyntax @class, ISymbol classSymbol)
    {
        Class = @class;
        ClassSymbol = classSymbol;
    }

    public ClassDeclarationSyntax Class { get; }
    public ISymbol ClassSymbol { get; }

    public ICollection<(PropertyDeclarationSyntax property, ISymbol symbol)> StartBy { get; } = new HashSet<(PropertyDeclarationSyntax, ISymbol)>();
    public ICollection<(PropertyDeclarationSyntax property, ISymbol symbol)> RequestTimeout { get; } = new HashSet<(PropertyDeclarationSyntax, ISymbol)>();
    public ICollection<(PropertyDeclarationSyntax property, ISymbol symbol)> Events { get; } = new HashSet<(PropertyDeclarationSyntax, ISymbol)>();
}