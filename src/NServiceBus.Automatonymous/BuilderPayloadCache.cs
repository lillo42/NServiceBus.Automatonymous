using System;
using GreenPipes;
using GreenPipes.Payloads;
using NServiceBus.ObjectBuilder;

namespace NServiceBus.Automatonymous
{
    public class BuilderPayloadCache : IPayloadCache
    {
        private readonly IBuilder _builder;
        private readonly ListPayloadCache _cache;

        public BuilderPayloadCache(IBuilder builder, 
            ListPayloadCache cache)
        {
            _builder = builder;
            _cache = cache;
        }

        public bool HasPayloadType(Type payloadType)
        {
            return _cache.HasPayloadType(payloadType) || AutomatonymousFeature.Container.HasComponent(payloadType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
        {
            if (_cache.TryGetPayload(out payload))
            {
                return true;
            }
            
            if (AutomatonymousFeature.Container.HasComponent(typeof(TPayload)))
            {
                payload = _builder.Build<TPayload>();
                return true;
            }

            return false;
        }

        public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory) where T : class
        {
            if (_cache.HasPayloadType(typeof(T)))
            {
                return _cache.GetOrAddPayload(payloadFactory);
            }
            
            if (AutomatonymousFeature.Container.HasComponent(typeof(T)))
            {
                return _builder.Build<T>();
            }
            
            return _cache.GetOrAddPayload(payloadFactory);
        }

        public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory) where T : class
        {
            return _cache.AddOrUpdatePayload(addFactory, updateFactory);
        }
    }
}