using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NServiceBus.Automatonymous.SourceGeneration;

internal class NServiceBusSagaFactory
{
    public static ClassBuilder Create(SagaInformation saga)
    {
        var className = $"{saga.Class.Identifier.Text}NServiceBusSaga";
        var builder = new ClassBuilder(new StringBuilder())
            .SetClassName(className)
            .SetBaseType($"NServiceBusSaga<{saga.Class.Identifier.Text}, {saga.ClassSymbol.Name}>")
            .SetNamespace("NServiceBus.Automatonymous.Generated")
            
            .AddUsing("System.Threading.Tasks")
            .AddUsing("NServiceBus")
            .AddUsing("NServiceBus.Automatonymous")
            .AddUsing("NServiceBus.ObjectBuilder")
            .AddUsing(saga.ClassSymbol.ContainingNamespace.ToDisplayString())
            
            
            .AddMethod($@"
public {className}({saga.Class.Identifier.Text} stateMachine, IBuilder builder)
    : base(stateMachine, builder)
{{
    
}}
")
            .AddUsing(saga.StartBy.Select(x => x.symbol.ContainingNamespace.ToDisplayString()))
            .AddInterfaces(saga.StartBy.Select(x => $"$IAmStartedByMessages<{x.symbol.Name}>"))
            .AddMethods(saga.StartBy.Select(x => CreateHandler(x.property, x.symbol)))
            
            .AddUsing(saga.RequestTimeout.Select(x => x.symbol.ContainingNamespace.ToDisplayString()))
            .AddInterfaces(saga.RequestTimeout.Select(x => $"IHandleTimeouts<{x.symbol.Name}>"))
            .AddMethods(saga.RequestTimeout.Select(x => CreateTimeoutHandler(x.property, x.symbol)))
            
            .AddUsing(saga.Events.Select(x => x.symbol.ContainingNamespace.ToDisplayString()))
            .AddInterfaces(saga.Events.Select(x => $"IHandleMessages<{x.symbol.Name}>"))
            .AddMethods(saga.Events.Select(x => CreateHandler(x.property, x.symbol)))
            ;

        return builder;

        static string CreateHandler(PropertyDeclarationSyntax propertyDeclarationSyntax, ISymbol symbol)
            => $@"
public Task Handle({symbol.Name} message, IMessageHandlerContext context)
    => Execute(message, context, StateMachine.{propertyDeclarationSyntax.Identifier.Text});
";
        
        static string CreateTimeoutHandler(PropertyDeclarationSyntax propertyDeclarationSyntax, ISymbol symbol)
            => $@"
public Task Timeout({symbol.Name} message, IMessageHandlerContext context)
    => Execute(message, context, StateMachine.{propertyDeclarationSyntax.Identifier.Text});
";
    }
}