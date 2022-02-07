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
//
//     private class Parse
//     {
//         private readonly GeneratorExecutionContext _executionContext;
//         private readonly INamedTypeSymbol _nServiceBusStateMachineContextSymbol;
//         private readonly INamedTypeSymbol _eventSymbol;
//         private readonly INamedTypeSymbol _startStateMachine;
//         private readonly INamedTypeSymbol _timeoutEvent;
//         private readonly INamedTypeSymbol _schedulerEvent;
//
//         public Parse(GeneratorExecutionContext executionContext)
//         {
//                 
//             _executionContext = executionContext;
//             _nServiceBusStateMachineContextSymbol = _executionContext.Compilation.GetTypeByMetadataName("NServiceBus.Automatonymous.NServiceBusStateMachine`1")!;
//             _eventSymbol = _executionContext.Compilation.GetTypeByMetadataName("Automatonymous.Event`1")!;
//             _startStateMachine = _executionContext.Compilation.GetTypeByMetadataName("NServiceBus.Automatonymous.StartStateMachineAttribute")!;
//             _timeoutEvent = _executionContext.Compilation.GetTypeByMetadataName("NServiceBus.Automatonymous.TimeoutEventAttribute")!;
//             _schedulerEvent = _executionContext.Compilation.GetTypeByMetadataName("NServiceBus.Automatonymous.Schedule`2")!;
//         }
//
//         private static DiagnosticDescriptor StartStateMachineAttributeNotFound { get; } = new(
//             id: "NSBA001",
//             title: "StartStateMachineAttribute not found.",
//             messageFormat: "StartStateMachineAttribute not found for type '{0}'.",
//             category: Const.NServiceBusAutomatonymousSourceGeneration,
//             defaultSeverity: DiagnosticSeverity.Error,
//             isEnabledByDefault: true);
//             
//         private static DiagnosticDescriptor StateMachineShouldHaveOnlyOneConstructor { get; } = new(
//             id: "NSBA002",
//             title: "StateMachine should have only one constructor.",
//             messageFormat: "StateMachine should have only one constructor for type '{0}'.",
//             category: Const.NServiceBusAutomatonymousSourceGeneration,
//             defaultSeverity: DiagnosticSeverity.Error,
//             isEnabledByDefault: true);
//             
//         private static DiagnosticDescriptor StateMachineConstructorShouldBeParameterless { get; } = new(
//             id: "NSBA003",
//             title: "StateMachine constructor should be parameter less.",
//             messageFormat: "StateMachine constructor should be parameter less for type '{0}'.",
//             category: Const.NServiceBusAutomatonymousSourceGeneration,
//             defaultSeverity: DiagnosticSeverity.Error,
//             isEnabledByDefault: true);
//
//         public NServiceBusSagaClassBuilder? CreateSagaBuilder(ClassDeclarationSyntax classDeclarationSyntax)
//         {
//             var compilation = _executionContext.Compilation;
//             var compilationUnitSyntax = classDeclarationSyntax.FirstAncestorOrSelf<CompilationUnitSyntax>()!;
//             var compilationSemanticModel = compilation.GetSemanticModel(compilationUnitSyntax.SyntaxTree);
//                 
//             if (!DerivesFrom(classDeclarationSyntax, _nServiceBusStateMachineContextSymbol,
//                     compilationSemanticModel, out var @base))
//             {
//                 return null;
//             }
//
//             var startByEvents = new List<PropertyDeclarationSyntax>();
//             var timeoutEvents = new List<PropertyDeclarationSyntax>();
//             var events = new List<PropertyDeclarationSyntax>();
//             var scheduler = new List<PropertyDeclarationSyntax>();
//
//             foreach (var propertyDeclarationSyntax in classDeclarationSyntax.Members
//                          .Where(x => x.Kind() == SyntaxKind.PropertyDeclaration && x.Modifiers.Any(modifier => modifier.Kind() == SyntaxKind.PublicKeyword))
//                          .Cast<PropertyDeclarationSyntax>())
//             {
//                 var genericNameSyntax = propertyDeclarationSyntax.DescendantNodes().OfType<GenericNameSyntax>().FirstOrDefault();
//                 var isSchedulerMessage = DerivesFrom(genericNameSyntax, _schedulerEvent, compilationSemanticModel);
//                 if (!DerivesFrom(genericNameSyntax, _eventSymbol, compilationSemanticModel) && !isSchedulerMessage)
//                 {
//                     continue;
//                 }
//                     
//                 if (HasAttribute(propertyDeclarationSyntax, _startStateMachine, compilationSemanticModel))
//                 {
//                     startByEvents.Add(propertyDeclarationSyntax);
//                 }
//                 else if (HasAttribute(propertyDeclarationSyntax, _timeoutEvent, compilationSemanticModel))
//                 {
//                     timeoutEvents.Add(propertyDeclarationSyntax);
//                 }
//                 else if (isSchedulerMessage)
//                 {
//                     scheduler.Add(propertyDeclarationSyntax);
//                 }
//                 else
//                 {
//                     events.Add(propertyDeclarationSyntax);
//                 }
//             }
//
//             if (startByEvents.Count == 0)
//             {
//                 _executionContext.ReportDiagnostic(Diagnostic.Create(StartStateMachineAttributeNotFound, Location.None, classDeclarationSyntax.Identifier.Text));
//                 return null;
//             }
//
//             if (events.Count == 0)
//             {
//                 _executionContext.ReportDiagnostic(Diagnostic.Create(StartStateMachineAttributeNotFound, Location.None, classDeclarationSyntax.Identifier.Text));
//                 return null;
//             }
//
//             var state = compilationSemanticModel.GetSymbolInfo(@base.TypeArgumentList.Arguments[0]).Symbol!;
//
//             var startByEventsGenericArgumentSymbol = startByEvents
//                 .Select(x => GetGenericParameterSymbol(x, compilationSemanticModel))
//                 .ToList();
//                 
//             var eventGenericArgumentSymbol = events
//                 .Select(x => GetGenericParameterSymbol(x, compilationSemanticModel))
//                 .ToList();
//                 
//             var timeoutEventsGenericArgumentSymbol = timeoutEvents
//                 .Select(x => GetGenericParameterSymbol(x, compilationSemanticModel))
//                 .ToList();
//                 
//             var schedulersGenericArgumentSymbol = scheduler
//                 .Select(x => GetGenericParameterSymbol(x, compilationSemanticModel, 1))
//                 .ToList();
//                 
//             return new NServiceBusSagaClassBuilder()
//                 .SetName($"{classDeclarationSyntax.Identifier.Text}NServiceBusSaga")
//                 .SetNamespace("NServiceBus.Automatonymous.Generated")
//                 .AddUsing("System.Threading.Tasks")
//
//                 .AddUsing("NServiceBus.Automatonymous")
//                 .AddUsing("NServiceBus.ObjectBuilder")
//                 .AddUsing(state.ContainingNamespace.ToDisplayString())
//                 .SetBaseType($"NServiceBusSaga<{classDeclarationSyntax.Identifier.Text}, {state.Name}>")
//                     
//                 .AddMethod($@"public {classDeclarationSyntax.Identifier.Text}NServiceBusSaga({classDeclarationSyntax.Identifier.Text} stateMachine, IBuilder builder)
//   : base(stateMachine, builder)
// {{
// }}") 
//
//                 .AddUsing("NServiceBus")
//                 .AddUsing(startByEventsGenericArgumentSymbol.Select(x => x!.ContainingNamespace.ToDisplayString()))
//                 .AddInterfaces(startByEventsGenericArgumentSymbol.Select(x => $"IAmStartedByMessages<{x!.Name}>"))
//                 .AddMethods(startByEvents.Zip(startByEventsGenericArgumentSymbol, CreateHandler!))
//                     
//                 .AddUsing(eventGenericArgumentSymbol.Select(x => x!.ContainingNamespace.ToDisplayString()))
//                 .AddInterfaces(eventGenericArgumentSymbol.Select(x => $"IHandleMessages<{x!.Name}>"))
//                 .AddMethods(events.Zip(eventGenericArgumentSymbol, CreateHandler!))
//                 
//                 .AddUsing(timeoutEventsGenericArgumentSymbol.Select(x => x!.ContainingNamespace.ToDisplayString()))
//                 .AddInterfaces(timeoutEventsGenericArgumentSymbol.Select(x => $"IHandleTimeouts<{x!.Name}>"))
//                 .AddMethods(timeoutEvents.Zip(timeoutEventsGenericArgumentSymbol, CreateTimeoutHandler!))
//
//                 .AddUsing(schedulersGenericArgumentSymbol.Select(x => x!.ContainingNamespace.ToDisplayString()))
//                 .AddInterfaces(schedulersGenericArgumentSymbol.Select(x => $"IHandleMessages<{x!.Name}>"))
//                 .AddMethods(scheduler.Zip(schedulersGenericArgumentSymbol, CreateScheduler!));
//         }
//             
//         private static bool HasAttribute(SyntaxNode propertyDeclarationSyntax, ISymbol attribute, SemanticModel compilationSemanticModel)
//         {
//             foreach (var attributeSyntax in propertyDeclarationSyntax.DescendantNodes().OfType<AttributeSyntax>())
//             {
//                 var symbolInfo = compilationSemanticModel.GetSymbolInfo(attributeSyntax);
//
//                 if (symbolInfo.Symbol is IMethodSymbol methodSymbol 
//                     && methodSymbol.ContainingType.Equals(attribute, SymbolEqualityComparer.Default))
//                 { 
//                     return true;
//                 }
//
//                 foreach (var candidateSymbol in symbolInfo.CandidateSymbols)
//                 {
//                     if (candidateSymbol.ContainingType.Equals(attribute, SymbolEqualityComparer.Default))
//                     {
//                         return true;
//                     }
//                 }
//             }
//
//             return false;
//         }
//
//         private static string CreateHandler(PropertyDeclarationSyntax propertyDeclarationSyntax, ISymbol symbol)
//             => $@"public Task Handle({symbol.Name} message, IMessageHandlerContext context)
// {{
//     return Execute(message, context, StateMachine.{propertyDeclarationSyntax.Identifier.Text});
// }}";
//             
//         private static string CreateTimeoutHandler(PropertyDeclarationSyntax propertyDeclarationSyntax, ISymbol symbol)
//             => $@"public Task Timeout({symbol.Name} message, IMessageHandlerContext context)
// {{
//     return Execute(message, context, StateMachine.{propertyDeclarationSyntax.Identifier.Text});
// }}";
//             
//         private static string CreateScheduler(PropertyDeclarationSyntax propertyDeclarationSyntax, ISymbol symbol)
//             => $@"public Task Handle({symbol.Name} message, IMessageHandlerContext context)
// {{
//     return Execute(message, context, StateMachine.{propertyDeclarationSyntax.Identifier.Text}.AnyReceived);
// }}";
//             
//         private static bool DerivesFrom(BaseTypeDeclarationSyntax? classDeclarationSyntax, ISymbol extendedSymbol, 
//             SemanticModel compilationSemanticModel, [NotNullWhen(true)]out GenericNameSyntax? genericNameSyntax)
//         {
//             genericNameSyntax = null;
//             var baseTypeSyntaxList = classDeclarationSyntax?.BaseList?.Types;
//             if (baseTypeSyntaxList == null)
//             {
//                 return false;
//             }
//
//             foreach (var baseTypeSyntax in baseTypeSyntaxList)
//             {
//                 if (compilationSemanticModel.GetSymbolInfo(baseTypeSyntax.Type).Symbol is INamedTypeSymbol candidate 
//                     && extendedSymbol.Equals(candidate.ConstructedFrom, SymbolEqualityComparer.Default))
//                 {
//                     genericNameSyntax = (GenericNameSyntax) baseTypeSyntax.Type;
//                     return true;
//                 }
//             }
//                 
//             return false;
//         }
//             
//         private static bool DerivesFrom(SyntaxNode? expression, ISymbol extendedSymbol, SemanticModel compilationSemanticModel)
//         {
//             if (expression == null)
//             {
//                 return false;
//             }
//                 
//             return compilationSemanticModel.GetSymbolInfo(expression).Symbol is INamedTypeSymbol candidate &&
//                    extendedSymbol.Equals(candidate.ConstructedFrom, SymbolEqualityComparer.Default);
//         }
//
//         private static ISymbol? GetGenericParameterSymbol(PropertyDeclarationSyntax propertyDeclarationSyntax, SemanticModel compilationSemanticModel, int argument = 0)
//         {
//             var genericNameSyntax =  propertyDeclarationSyntax.DescendantNodes().OfType<GenericNameSyntax>().FirstOrDefault();
//             return genericNameSyntax == null ? null : compilationSemanticModel.GetSymbolInfo(genericNameSyntax.TypeArgumentList.Arguments[argument]).Symbol;
//         }
//     }
}