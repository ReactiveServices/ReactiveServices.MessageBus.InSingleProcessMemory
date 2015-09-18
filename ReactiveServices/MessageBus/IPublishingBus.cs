using System;
using System.Collections.Generic;

namespace ReactiveServices.MessageBus
{
    public interface IPublishingBus : IMessageBus
    {
        /// <summary>
        /// Publish the given message
        /// </summary>
        /// <remarks>Publishing of persistent messages are only confirmed after they are stored to disk, so the confirmation can take a long time to come</remarks>
        void Publish(
            Type messageType, object message, 
            StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan));
        /// <summary>
        /// Publish the given message in a given topic
        /// </summary>
        /// <remarks>Publishing of persistent messages are only confirmed after they are stored to disk, so the confirmation can take a long time to come</remarks>
        void Publish(
            Type messageType, TopicId topicId, object message, 
            StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan));
        /// <summary>
        /// Publish the given message
        /// </summary>
        /// <remarks>Publishing of persistent messages are only confirmed after they are stored to disk, so the confirmation can take a long time to come</remarks>
        void Publish<TMessage>(
            TMessage message, 
            StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan)) where TMessage : class;
        /// <summary>
        /// Publish the given message in a given topic
        /// </summary>
        /// <remarks>Publishing of persistent messages are only confirmed after they are stored to disk, so the confirmation can take a long time to come</remarks>
        void Publish<TMessage>(
            TopicId topicId, TMessage message, 
            StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan)) where TMessage : class;
        /// <summary>
        /// Delete the exchange created to receive messages from a given publishing
        /// </summary>
        void DeletePublishingExchange(Type type);
    }
}
