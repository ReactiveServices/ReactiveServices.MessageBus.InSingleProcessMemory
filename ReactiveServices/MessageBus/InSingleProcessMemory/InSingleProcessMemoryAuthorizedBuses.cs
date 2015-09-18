using ReactiveServices.Authorization;
using System;
using System.Collections.Generic;

namespace ReactiveServices.MessageBus.InSingleProcessMemory
{
    class InSingleProcessMemoryAuthorizer
    {
        public static void Verify(AuthorizedMessageOperation operation, Type messageType, object message, Dictionary<string, string> headers)
        {
            if (!headers.ContainsKey(Authorizer.AuthenticationTokenKey))
                throw new AuthorizationException(Authorizer.AuthorizationDomain, operation.ToString(), messageType.FullName, AuthorizationError.InvalidAuthenticationToken);

            var authenticationToken = headers[Authorizer.AuthenticationTokenKey];
            Authorizer.Verify(operation.ToString(), messageType.FullName, authenticationToken);
        }
    }

    public class InSingleProcessMemoryAuthorizedSubscription : InSingleProcessMemorySubscription
    {
        public InSingleProcessMemoryAuthorizedSubscription(IInSingleProcessMemoryMessageBus messageBus, AuthorizedMessageOperation operation, SubscriptionId subscriptionId, TopicId topicId, Type messageType, string queueName, SubscriptionMode subscriptionMode, bool acceptMessagesOlderThanSubscriptionTime, Action<object> messageHandler, Action<object, object> messageHandlerWithProperties, Action<object, Dictionary<string, string>> messageHandlerWithHeaders, Action<object, object, Dictionary<string, string>> messageHandlerWithPropertiesAndHeaders)
            : base(messageBus, subscriptionId, topicId, messageType, queueName, subscriptionMode, acceptMessagesOlderThanSubscriptionTime, messageHandler, messageHandlerWithProperties, messageHandlerWithHeaders, messageHandlerWithPropertiesAndHeaders)
        {
            Operation = operation;
        }

        private readonly AuthorizedMessageOperation Operation;

        protected override InSingleProcessMemoryConsumer NewConsumer(IInSingleProcessMemoryMessageBus messageBus)
        {
            return new InSingleProcessMemoryAuthorizedConsumer(this, Operation, messageBus);
        }
    }

    public class InSingleProcessMemoryAuthorizedConsumer : InSingleProcessMemoryConsumer
    {
        internal InSingleProcessMemoryAuthorizedConsumer(InSingleProcessMemorySubscription subscription, AuthorizedMessageOperation operation, IInSingleProcessMemoryMessageBus messageBus)
            : base(subscription, messageBus)
        {
            Operation = operation;
        }

        private readonly AuthorizedMessageOperation Operation;

        protected override void ExecuteSubscriptionMessageHandler(IBasicProperties properties, object message, Dictionary<string, string> headers)
        {
            var messageType = message.GetType();

            InSingleProcessMemoryAuthorizer.Verify(Operation, messageType, message, headers);

            base.ExecuteSubscriptionMessageHandler(properties, message, headers);
        }
    }

    public class InSingleProcessMemoryAuthorizedPublishingBus : InSingleProcessMemoryPublishingBus, IAuthorizedPublishingBus
    {
        protected override void TryPublish(
            Type messageType, TopicId topicId, object message, StorageType storageType,
            bool waitForPublishConfirmation, TimeSpan publishConfirmationTimeout,
            Dictionary<string, string> headers, TimeSpan expiration)
        {
            InSingleProcessMemoryAuthorizer.Verify(AuthorizedMessageOperation.Publish, messageType, message, headers);

            base.TryPublish(messageType, topicId, message, storageType, waitForPublishConfirmation,
                publishConfirmationTimeout, headers, expiration);
        }
    }

    public class InSingleProcessMemoryAuthorizedSubscriptionBus : InSingleProcessMemorySubscriptionBus, IAuthorizedSubscriptionBus
    {
        protected override InSingleProcessMemorySubscription NewSubscribeSubscription(Type messageType, SubscriptionId subscriptionId, TopicId topicId, SubscriptionMode subscriptionMode,
            bool acceptMessagesOlderThanSubscriptionTime, Action<object> messageHandler, string queueName)
        {
            return new InSingleProcessMemoryAuthorizedSubscription(
                this, AuthorizedMessageOperation.Subscribe, subscriptionId, topicId, messageType, queueName,
                subscriptionMode, acceptMessagesOlderThanSubscriptionTime, messageHandler, null, null, null
            );
        }
    }

    public class InSingleProcessMemoryAuthorizedRequestBus : InSingleProcessMemoryRequestBus, IAuthorizedRequestBus
    {
        protected override InSingleProcessMemorySubscription NewResponseSubscription(Type responseType, string replyQueueName, Action<object, object> messageHandler)
        {
            return new InSingleProcessMemoryAuthorizedSubscription(
                this, AuthorizedMessageOperation.Request, null, TopicId.None, responseType, replyQueueName,
                SubscriptionMode.Shared, true, null, messageHandler, null, null
            );
        }

        protected override void TryPublishRequest(
            Type requestType, IRequest request, string correlationId, string replyQueueName,
            SubscriptionId subscriptionId, Dictionary<string, string> headers, TimeSpan expiration)
        {
            InSingleProcessMemoryAuthorizer.Verify(AuthorizedMessageOperation.Request, requestType, request, headers);

            base.TryPublishRequest(requestType, request, correlationId, replyQueueName, subscriptionId, headers, expiration);
        }
    }

    public class InSingleProcessMemoryAuthorizedResponseBus : InSingleProcessMemoryResponseBus, IAuthorizedResponseBus
    {
        protected override InSingleProcessMemorySubscription NewRequestSubscription(Type requestType, string requestQueueName, Action<object, object> messageHandler)
        {
            return new InSingleProcessMemoryAuthorizedSubscription(
                this, AuthorizedMessageOperation.Respond, null, TopicId.None, requestType, requestQueueName,
                SubscriptionMode.Shared, true, null, messageHandler, null, null
            );
        }

        protected override void TryPublishResponse(
            Type responseType, string replyQueueName, string correlationId, IResponse response,
            Dictionary<string, string> headers, TimeSpan expiration)
        {
            InSingleProcessMemoryAuthorizer.Verify(AuthorizedMessageOperation.Respond, responseType, response, headers);

            base.TryPublishResponse(responseType, replyQueueName, correlationId, response, headers, expiration);
        }
    }

    public class InSingleProcessMemoryAuthorizedSendingBus : InSingleProcessMemorySendingBus, IAuthorizedSendingBus
    {
        protected override void TrySend(
            Type messageType, object message, SubscriptionId subscriptionId, StorageType storageType,
            bool waitForPublishConfirmation, TimeSpan publishConfirmationTimeout,
            Dictionary<string, string> headers, TimeSpan expiration)
        {
            InSingleProcessMemoryAuthorizer.Verify(AuthorizedMessageOperation.Send, messageType, message, headers);

            base.TrySend(messageType, message, subscriptionId, storageType, waitForPublishConfirmation, publishConfirmationTimeout, headers, expiration);
        }
    }

    public class InSingleProcessMemoryAuthorizedReceivingBus : InSingleProcessMemoryReceivingBus, IAuthorizedReceivingBus
    {
        protected override InSingleProcessMemorySubscription NewReceivingSubscription(Type messageType, SubscriptionId subscriptionId, SubscriptionMode subscriptionMode,
            bool acceptMessagesOlderThanSubscriptionTime, Action<object> messageHandler, string queueName)
        {
            return new InSingleProcessMemoryAuthorizedSubscription(
                this, AuthorizedMessageOperation.Receive, subscriptionId, TopicId.None, messageType, queueName,
                subscriptionMode, acceptMessagesOlderThanSubscriptionTime, messageHandler, null, null, null
            );
        }
    }
}
