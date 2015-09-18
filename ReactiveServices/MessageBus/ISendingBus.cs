using System;
using System.Collections.Generic;

namespace ReactiveServices.MessageBus
{
    public interface ISendingBus : IMessageBus
    {
        /// <summary>
        /// Send a message directly to a given subscription, without passing through an exchange
        /// </summary>
        /// <remarks>Publishing of persistent messages are only confirmed after they are stored to disk, so the confirmation can take a long time to come</remarks>
        void Send<TMessage>(
            TMessage message, SubscriptionId subscriptionId, 
            StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
            where TMessage : class;

        /// <summary>
        /// Send a message directly to a given subscription, without passing through an exchange
        /// </summary>
        /// <remarks>Publishing of persistent messages are only confirmed after they are stored to disk, so the confirmation can take a long time to come</remarks>
        void Send(
            Type messageType, object message, SubscriptionId subscriptionId, 
            StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan));
    }
}
