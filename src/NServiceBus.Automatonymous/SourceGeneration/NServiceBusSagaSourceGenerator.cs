using Microsoft.CodeAnalysis;

namespace NServiceBus.Automatonymous.SourceGeneration;

/// <summary>
/// The<see cref="NServiceBusSaga{TStateMachine,TState}"/> generator.
/// </summary>
[Generator]
public class NServiceBusSagaSourceGenerator : ISourceGenerator
{
    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new StateMachineReceiver());
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not StateMachineReceiver receiver || receiver.Sagas.Count == 0)
        {
            return;
        }
        
        foreach (var sagaInformation in receiver.Sagas)
        {
            var builder = NServiceBusSagaFactory.Create(sagaInformation);
            context.AddSource($"{builder.ClassName}.generated.cs", builder.ToString());
        }
    }
}