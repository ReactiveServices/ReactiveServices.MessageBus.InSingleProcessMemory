using System;

namespace ReactiveServices.MessageBus
{
    public interface IReceivingBus : IMessageBus
    {
        /// <summary>
        /// Register a callback to be executed when a message of the given type is received
        /// </summary>
        void Receive(
            Type messageType, SubscriptionId subscriptionId, 
            Action<object> messageHandler, 
            SubscriptionMode subscriptionMode = SubscriptionMode.Shared);
        /// <summary>
        /// Register a callback to be executed when a message of the given type is received
        /// </summary>
        void Receive<TMessage>(
            SubscriptionId subscriptionId, 
            Action<object> messageHandler, 
            SubscriptionMode subscriptionMode = SubscriptionMode.Shared) where TMessage : class, new();
        /// <summary>
        /// Remove the subscription
        /// </summary>
        /// <remarks>
        /// If the subscription was created with SubscriptionMode == SubscriptionMode.Exclusive, then the Subscription Queue will be deleted too
        /// </remarks>
        void RemoveSubscription(Type messageType, SubscriptionId subscriptionId);
        /// <summary>
        /// Remove any subscription with the given subscription id, for any message type
        /// </summary>
        /// <remarks>
        /// If the subscription was created with SubscriptionMode == SubscriptionMode.Exclusive, then the Subscription Queue will be deleted too
        /// </remarks>
        void RemoveSubscriptions(SubscriptionId subscriptionId);
        /// <summary>
        /// Delete the queue created to redirect messages to the given subscription id
        /// </summary>
        void DeleteSubscriptionQueue(Type messageType, SubscriptionId subscriptionId);
        /// <summary>
        /// Indicates if there is an active subscription with the given subscription id
        /// </summary>
        bool IsListenningTo(Type messageType, SubscriptionId subscriptionId);
        /// <summary>
        /// Creates a subscription queue for the given message type and subscription id to receive publications before an actual subscription happens
        /// </summary>
        void PrepareSubscriptionTo<TMessage>(SubscriptionId subscriptionId) where TMessage : class, new();
        /// <summary>
        /// Creates a subscription queue for the given message type and subscription id to receive publications before an actual subscription happens
        /// </summary>
        void PrepareSubscriptionTo(Type messageType, SubscriptionId subscriptionId);
    }
}
