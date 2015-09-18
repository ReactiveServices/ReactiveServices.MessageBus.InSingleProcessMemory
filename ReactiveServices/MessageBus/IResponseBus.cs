using System;
using System.Collections.Generic;

namespace ReactiveServices.MessageBus
{
    public interface IResponseBus : IMessageBus
    {
        /// <summary>
        /// Register a callback to be executed to produce a response for a given request type
        /// </summary>
        void StartRespondingTo<TRequest, TResponse>(
            Func<TRequest, TResponse> requestReceivedCallback, 
            SubscriptionId subscriptionId = null,
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
            where TRequest : class, IRequest
            where TResponse : class, IResponse; 
        
        /// <summary>
        /// Register a callback to be executed to produce a response for a given request type
        /// </summary>
        void StartRespondingTo(
            Type requestType, Type responseType, 
            Func<IRequest, IResponse> requestReceivedCallback, 
            SubscriptionId subscriptionId = null,
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan));

        /// <summary>
        /// Unregister the callback registered to respond to a given request type
        /// </summary>
        void StopRespondingTo<TRequest>(SubscriptionId subscriptionId = null)
            where TRequest : class, IRequest;

        /// <summary>
        /// Unregister the callback registered to respond to a given request type
        /// </summary>
        void StopRespondingTo(Type requestType, SubscriptionId subscriptionId = null);
    }
}
