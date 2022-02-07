using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NServiceBus.Automatonymous.SourceGeneration;

internal class StateMachineReceiver : ISyntaxContextReceiver
{
    public List<SagaInformation> Sagas { get; } = new();
    private SagaInformation? _saga;
    private PropertyType? _propertyType;
    
    private enum PropertyType
    {
        StartAt,
        Event,
        RequestTimeout
    }
    
    private static bool ExtendFrom(GeneratorSyntaxContext context, ClassDeclarationSyntax @class, INamedTypeSymbol typeSymbol)
    {
        var baseTypeSyntaxList = @class.BaseList?.Types;
        if (baseTypeSyntaxList == null)
        {
            return false;
        }

        foreach (var baseTypeSyntax in baseTypeSyntaxList)
        {
            if (context.SemanticModel.GetSymbolInfo(baseTypeSyntax.Type).Symbol is INamedTypeSymbol candidate
                && (typeSymbol.Equals(candidate, SymbolEqualityComparer.Default) || typeSymbol.Equals(candidate.ConstructedFrom, SymbolEqualityComparer.Default)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSaga(GeneratorSyntaxContext context, ClassDeclarationSyntax @class)
        => ExtendFrom(context, @class,
            context.SemanticModel.Compilation.GetTypeByMetadataName("NServiceBus.Automatonymous.NServiceBusStateMachine`1")!);

    private static bool IsInitially(SyntaxNode node)
    {
        while (node != null && node is not ConstructorDeclarationSyntax)
        {
            if (node is InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "Initially" } })
            {
                return true;
            }
            
            node = node.Parent;
        }

        return false;
    }
    
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is ClassDeclarationSyntax classDeclarationSyntax)
        {
            if (IsSaga(context, classDeclarationSyntax))
            {
                _saga = new SagaInformation(classDeclarationSyntax, context.SemanticModel.GetSymbolInfo(classDeclarationSyntax).Symbol!);
                Sagas.Add(_saga);
            }
            else
            {
                _saga = null;
            }
        }

        if (_saga != null)
        {
            
            switch (context.Node)
            {
                // TODO: Change to method ref
                case IdentifierNameSyntax { Identifier.Text: "When" }:
                    _propertyType = IsInitially(context.Node) ?  PropertyType.StartAt : PropertyType.Event;
                    break;
                case IdentifierNameSyntax { Identifier.Text: "RequestTimeout" }:
                    _propertyType = PropertyType.RequestTimeout;
                    break;
                case IdentifierNameSyntax identifier:
                {
                    if (_propertyType != null)
                    {
                        var property = _saga.Class.ChildNodes().FirstOrDefault(x => x is PropertyDeclarationSyntax p && p.Identifier.Text == identifier.Identifier.Text);
                        if (property == null)
                        {
                            return;
                        }
                        var symbol = context.SemanticModel.GetSymbolInfo(property.DescendantNodes().OfType<GenericNameSyntax>().First()).Symbol;
                        if (_propertyType == PropertyType.StartAt)
                        {
                            _saga.StartBy.Add(((property as PropertyDeclarationSyntax)!, symbol!));
                            _propertyType = null;
                        }
                        else if (_propertyType == PropertyType.Event)
                        {
                            _saga.Events.Add(((property as PropertyDeclarationSyntax)!, symbol!));
                            _propertyType = null;
                        }
                    }

                    break;
                }
                case SimpleLambdaExpressionSyntax simpleLambdaExpression when _propertyType != null:
                {
                    TypeInfo? type = null;
                    if (simpleLambdaExpression.ExpressionBody is ObjectCreationExpressionSyntax objectCreation)
                    {
                        type = context.SemanticModel.GetTypeInfo(objectCreation);
                    }
                    if (simpleLambdaExpression.Block != null)
                    {
                        var block = simpleLambdaExpression.ChildNodes().FirstOrDefault(x => x is BlockSyntax);
                        var returnStatement = block?.ChildNodes().FirstOrDefault(x => x is ReturnStatementSyntax);
                        if (returnStatement != null)
                        {
                            type = context.SemanticModel.GetTypeInfo((returnStatement as ReturnStatementSyntax)!.Expression!);
                        }
                    }

                    if (type?.Type == null)
                    {
                        return;
                    }
                
                    var property = _saga.Class.ChildNodes().FirstOrDefault(x => x is PropertyDeclarationSyntax { Type: GenericNameSyntax genericNameSyntax } 
                        && genericNameSyntax.TypeArgumentList.Arguments.Count == 1 
                        && type.Value.Type!.Equals(context.SemanticModel.GetTypeInfo(genericNameSyntax.TypeArgumentList.Arguments[0]).Type, SymbolEqualityComparer.Default));
                    if (property != null)
                    {
                        var symbol = context.SemanticModel.GetSymbolInfo(property.DescendantNodes().OfType<GenericNameSyntax>().First()).Symbol;
                        _saga.RequestTimeout.Add(((property as PropertyDeclarationSyntax)!, symbol!));
                        _propertyType = null;
                    }

                    break;
                }
            }
        }
    }
}