using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactiveServices.MessageBus
{
    public interface IRequestBus : IMessageBus
    {
        /// <summary>
        /// Send a given request and waits for a response.
        /// </summary>
        TResponse Request<TRequest, TResponse>(
            TRequest request, 
            SubscriptionId subscriptionId = null, 
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
            where TRequest : class, IRequest
            where TResponse : class, IResponse;

        /// <summary>
        /// Send a given request and waits for a response.
        /// </summary>
        IResponse Request(
            Type requestType, Type responseType, IRequest request, 
            SubscriptionId subscriptionId = null,
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan));

        /// <summary>
        /// Send a given request and waits for a response.
        /// </summary>
        Task<TResponse> RequestAsync<TRequest, TResponse>(
            TRequest request, 
            SubscriptionId subscriptionId = null,
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
            where TRequest : class, IRequest
            where TResponse : class, IResponse;

        /// <summary>
        /// Send a given request and waits for a response.
        /// </summary>
        Task<IResponse> RequestAsync(
            Type requestType, Type responseType, IRequest request, 
            SubscriptionId subscriptionId = null,
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan));

        /// <summary>
        /// Delete the queue used for a given type of request
        /// </summary>
        void DeleteRequestQueue(Type requestType, SubscriptionId subscriptionId = null);
    }
}
