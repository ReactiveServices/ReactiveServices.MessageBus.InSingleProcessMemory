using System;
using System.Collections.Generic;

namespace ReactiveServices.MessageBus
{
    public abstract class Subscription : IDisposable
    {
        protected Subscription(
            SubscriptionId subscriptionId, TopicId topicId, Type messageType,
            string queueName,
            SubscriptionMode subscriptionMode,
            bool acceptMessagesOlderThanSubscriptionTime, 
            Action<object> messageHandler,
            Action<object, object> messageHandlerWithProperties,
            Action<object, Dictionary<string, string>> messageHandlerWithHeaders,
            Action<object, object, Dictionary<string, string>> messageHandlerWithPropertiesAndHeaders)
        {
            if ((messageHandler != null) && (messageHandlerWithProperties != null))
                throw new ArgumentException("MessageHandler and MessageHandlerWithProperties cannot be both assigned!");

            SubscriptionId = subscriptionId;
            TopicId = topicId;
            MessageType = messageType;
            SubscriptionMode = subscriptionMode;
            AcceptMessagesOlderThanSubscriptionTime = acceptMessagesOlderThanSubscriptionTime;
            MessageHandler = messageHandler;
            MessageHandlerWithHeaders = messageHandlerWithHeaders;
            MessageHandlerWithProperties = messageHandlerWithProperties;
            MessageHandlerWithPropertiesAndHeaders = messageHandlerWithPropertiesAndHeaders;
            QueueName = queueName;
        }

        public SubscriptionId SubscriptionId { get; private set; }
        public TopicId TopicId { get; private set; }
        public Type MessageType { get; private set; }
        public SubscriptionMode SubscriptionMode { get; private set; }
        public bool AcceptMessagesOlderThanSubscriptionTime { get; private set; }
        public Action<object> MessageHandler { get; private set; }
        public Action<object, Dictionary<string, string>> MessageHandlerWithHeaders { get; private set; }
        public Action<object, object> MessageHandlerWithProperties { get; private set; }
        public Action<object, object, Dictionary<string, string>> MessageHandlerWithPropertiesAndHeaders { get; private set; }
        public string QueueName { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
    }
}
