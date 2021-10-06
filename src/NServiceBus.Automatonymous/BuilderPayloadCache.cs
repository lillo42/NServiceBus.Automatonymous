using System;
using GreenPipes;
using GreenPipes.Payloads;
using NServiceBus.ObjectBuilder;

namespace NServiceBus.Automatonymous;

/// <summary>
/// The implementation <see cref="IPayloadCache"/> using the <see cref="ListPayloadCache"/> for local type
/// and <see cref="IBuilder"/> to resolve some types from dependency injector.
/// </summary>
public class BuilderPayloadCache : IPayloadCache
{
    private readonly IBuilder _builder;
    private readonly IConfigureComponents _configureComponents;
    private readonly ListPayloadCache _cache;

    /// <summary>
    /// Initialize new instance of <see cref="BuilderPayloadCache"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IBuilder"/>.</param>
    /// <param name="configureComponents">The <see cref="IConfigureComponents"/>.</param>
    /// <param name="cache">The <see cref="ListPayloadCache"/>.</param>
    public BuilderPayloadCache(IBuilder builder,
        IConfigureComponents configureComponents,
        ListPayloadCache cache)
    {
        _builder = builder;
        _cache = cache;
        _configureComponents = configureComponents;
    }

    /// <inheritdoc />
    public bool HasPayloadType(Type payloadType)
    {
        return _cache.HasPayloadType(payloadType) || _configureComponents.HasComponent(payloadType);
    }

    /// <inheritdoc />
    public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
    {
        if (_cache.TryGetPayload(out payload))
        {
            return true;
        }
            
        if (_configureComponents.HasComponent(typeof(TPayload)))
        {
            payload = _builder.Build<TPayload>();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory) where T : class
    {
        if (_cache.HasPayloadType(typeof(T)))
        {
            return _cache.GetOrAddPayload(payloadFactory);
        }
            
        if (_configureComponents.HasComponent<T>())
        {
            return _builder.Build<T>();
        }
            
        return _cache.GetOrAddPayload(payloadFactory);
    }
        
    /// <inheritdoc />
    public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory) where T : class
    {
        return _cache.AddOrUpdatePayload(addFactory, updateFactory);
    }
}