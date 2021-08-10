using System;
using Automatonymous.Activities;
using Automatonymous.Binders;
using GreenPipes;
using NServiceBus;

namespace Automatonymous
{
    public static class AutomatonymousExtensions
    {
        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, TMessage> createMessage,
            Action<BehaviorContext<TInstance, TData>, SendOptions>? configSendOptions = null)
            where TInstance : class
        {
            return binder.Add(new AsyncActivity<TInstance, TData>(context =>
            {
                var message = createMessage(context);

                var options = new SendOptions();
                configSendOptions?.Invoke(context, options);    
                
                return context.GetPayload<IMessageHandlerContext>()
                    .Send(message, options);
            }));
        } 
        
        
        public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, TMessage> createMessage,
            Action<BehaviorContext<TInstance, TData>, PublishOptions>? configSendOptions = null)
            where TInstance : class
        {
            return binder.Add(new AsyncActivity<TInstance, TData>(context =>
            {
                var message = createMessage(context);

                var options = new PublishOptions();
                configSendOptions?.Invoke(context, options);    
                
                return context.GetPayload<IMessageHandlerContext>()
                    .Publish(message, options);
            }));
        }  
        
        public static EventActivityBinder<TInstance, TMessage> RequestTimeout<TInstance, TMessage, TRequestMessage>(
            this EventActivityBinder<TInstance, TMessage> binder,
            Func<BehaviorContext<TInstance, TMessage>, TRequestMessage> createMessage, DateTime at)
            where TInstance : class, IContainSagaData
        {
            if (at.Kind == DateTimeKind.Unspecified)
            {
                throw new InvalidOperationException("Kind property of DateTime 'at' must be specified.");
            }
            
            return binder.SendAsync(createMessage, (context, opt) =>
            {
                opt.DoNotDeliverBefore(at);
                opt.RouteToThisEndpoint();
                opt.SetHeader(Headers.SagaId, context.Instance.Id.ToString());
                opt.SetHeader(Headers.IsSagaTimeoutMessage, bool.TrueString);
                opt.SetHeader(Headers.SagaType, context.GetPayload<Type>().AssemblyQualifiedName);
            });
        } 
        
        public static EventActivityBinder<TInstance, TMessage> RequestTimeout<TInstance, TMessage, TRequestMessage>(
            this EventActivityBinder<TInstance, TMessage> binder, DateTime at)
            where TInstance : class, IContainSagaData
            where TRequestMessage : new() 
            => binder.RequestTimeout(_ => new TRequestMessage(), at);
    }
}