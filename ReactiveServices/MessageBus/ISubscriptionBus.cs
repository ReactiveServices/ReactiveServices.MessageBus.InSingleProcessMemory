using System;
using System.Collections.Generic;

namespace ReactiveServices.MessageBus
{
    public interface ISubscriptionBus : IMessageBus
    {
        /// <summary>
        /// Register a callback to be executed when a message of the given type is received
        /// </summary>
        /// <param name="messageType">Type of the expected message</param>
        /// <param name="subscriptionId">Id of the subscription. If reused with the same message type, delivery will be performed in round robin style</param>
        /// <param name="messageHandler">Callcack executed when the messages are received</param>
        /// <param name="subscriptionMode">Indicates if the queue create for the subscription can be shared with other subscribers</param>
        void SubscribeTo(Type messageType, SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode = SubscriptionMode.Shared);
        /// <summary>
        /// Register a callback to be executed when a message of the given type is received on a given topic
        /// </summary>
        /// <param name="messageType">Type of the expected message</param>
        /// <param name="topicId">Indicates the topic where the messages should arive</param>
        /// <param name="subscriptionId">Id of the subscription. If reused with the same message type, delivery will be performed in round robin style</param>
        /// <param name="messageHandler">Callcack executed when the messages are received</param>
        /// <param name="subscriptionMode">Indicates if the queue create for the subscription can be shared with other subscribers</param>
        void SubscribeTo(Type messageType, TopicId topicId, SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode = SubscriptionMode.Shared);

        /// <summary>
        /// Register a callback to be executed when a message of the given type is received
        /// </summary>
        /// <typeparam name="TMessage">Type of the expected message</typeparam>
        /// <param name="subscriptionId">Id of the subscription. If reused with the same message type, delivery will be performed in round robin style</param>
        /// <param name="messageHandler">Callcack executed when the messages are received</param>
        /// <param name="subscriptionMode">Indicates if the queue create for the subscription can be shared with other subscribers</param>
        void SubscribeTo<TMessage>(SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode = SubscriptionMode.Shared) where TMessage : class, new();
        /// <summary>
        /// Register a callback to be executed when a message of the given type is received on a given topic
        /// </summary>
        /// <typeparam name="TMessage">Type of the expected message</typeparam>
        /// <param name="topicId">Indicates the topic where the messages should arive</param>
        /// <param name="subscriptionId">Id of the subscription. If reused with the same message type, delivery will be performed in round robin style</param>
        /// <param name="messageHandler">Callcack executed when the messages are received</param>
        /// <param name="subscriptionMode">Indicates if the queue create for the subscription can be shared with other subscribers</param>
        void SubscribeTo<TMessage>(TopicId topicId, SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode = SubscriptionMode.Shared) where TMessage : class, new();
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
        /// <summary>
        /// Creates a subscription queue for the given message type and subscription id to receive publications before an actual subscription happens
        /// </summary>
        void PrepareSubscriptionTo<TMessage>(TopicId topicId, SubscriptionId subscriptionId) where TMessage : class, new();
        /// <summary>
        /// Creates a subscription queue for the given message type and subscription id to receive publications before an actual subscription happens
        /// </summary>
        void PrepareSubscriptionTo(Type messageType, TopicId topicId, SubscriptionId subscriptionId);

        bool QueueExists(string queueName);

        /// <summary>
        /// Return the subscription ids for all queues bound to the given message type
        /// </summary>
        IEnumerable<SubscriptionId> AvailableSubscriptionsFor<TMessage>();
    }
}
