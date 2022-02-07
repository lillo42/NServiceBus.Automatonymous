using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NServiceBus.Automatonymous.SourceGeneration;

internal class SagaInformation
{
    public SagaInformation(ClassDeclarationSyntax @class, ISymbol stateSymbol)
    {
        Class = @class;
        StateSymbol = stateSymbol;
    }

    public ClassDeclarationSyntax Class { get; }
    public ISymbol StateSymbol { get; }

    public ICollection<(PropertyDeclarationSyntax property, ISymbol symbol)> StartBy { get; } = new HashSet<(PropertyDeclarationSyntax, ISymbol)>();
    public ICollection<(PropertyDeclarationSyntax property, ISymbol symbol)> RequestTimeout { get; } = new HashSet<(PropertyDeclarationSyntax, ISymbol)>();
    public ICollection<(PropertyDeclarationSyntax property, ISymbol symbol)> Events { get; } = new HashSet<(PropertyDeclarationSyntax, ISymbol)>();
}