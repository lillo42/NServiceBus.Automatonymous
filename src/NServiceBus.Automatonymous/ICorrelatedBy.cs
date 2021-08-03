namespace NServiceBus.Automatonymous
{
    public interface ICorrelatedBy
    {
        object? CorrelationId { get; }
    }
    
    public interface ICorrelatedBy<out TKey> : ICorrelatedBy
    {
        object? ICorrelatedBy.CorrelationId => CorrelationId;
        new TKey CorrelationId { get; }
    }
}