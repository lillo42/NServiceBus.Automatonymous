using System.Diagnostics.CodeAnalysis;

namespace NServiceBus.Automatonymous;

/// <summary>
/// Correlating message by the <see cref="CorrelationId"/>.
/// </summary>
/// <remarks>
/// When a message implement this interface we automatic bind the Correlation with the saga.
/// </remarks>
public interface ICorrelatedBy
{
    /// <summary>
    /// The CorrelationId.
    /// </summary>
    object? CorrelationId { get; }
}
    
/// <summary>
/// Correlating message by the <see cref="CorrelationId"/>.
/// </summary>
/// <typeparam name="TKey">The Correlation type.</typeparam>
/// <remarks>
/// When a message implement this interface we automatic bind the Correlation with the saga.
/// </remarks>
public interface ICorrelatedBy<out TKey> : ICorrelatedBy
{
    [ExcludeFromCodeCoverage]
    object? ICorrelatedBy.CorrelationId => CorrelationId;
        
    /// <summary>
    /// The CorrelationId.
    /// </summary>
    new TKey CorrelationId { get; }
}