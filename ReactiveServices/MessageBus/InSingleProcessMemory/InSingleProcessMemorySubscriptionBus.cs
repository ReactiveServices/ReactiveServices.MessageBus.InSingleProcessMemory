using NLog;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactiveServices.MessageBus.InSingleProcessMemory
{
    public class InSingleProcessMemorySubscriptionBus : InSingleProcessMemoryMessageBus, ISubscriptionBus
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Tuple<Type, SubscriptionId>, InSingleProcessMemorySubscription> ActiveSubscriptions = new Dictionary<Tuple<Type, SubscriptionId>, InSingleProcessMemorySubscription>();

        public bool IsListenningTo(Type messageType, SubscriptionId subscriptionId)
        {
            return ExistingSubscriptionFor(messageType, subscriptionId) != null;
        }

        protected bool IgnoreMessagesOlderThanSubscriptionTime { get; set; }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void SubscribeTo(Type messageType, SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode = SubscriptionMode.Shared)
        {
            TrySubscribeTo(messageType, subscriptionId, TopicId.Default, messageHandler, subscriptionMode);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void SubscribeTo(Type messageType, TopicId topicId, SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode = SubscriptionMode.Shared)
        {
            TrySubscribeTo(messageType, subscriptionId, topicId, messageHandler, subscriptionMode);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void SubscribeTo<TMessage>(SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode = SubscriptionMode.Shared)
            where TMessage : class, new()
        {
            TrySubscribeTo(typeof(TMessage), subscriptionId, TopicId.Default, messageHandler, subscriptionMode);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void SubscribeTo<TMessage>(TopicId topicId, SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode = SubscriptionMode.Shared)
            where TMessage : class, new()
        {
            TrySubscribeTo(typeof(TMessage), subscriptionId, topicId, messageHandler, subscriptionMode);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void RemoveSubscription(Type messageType, SubscriptionId subscriptionId)
        {
            var subscription = ExistingSubscriptionFor(messageType, subscriptionId);
            if (subscription != null)
                RemoveSubscription(subscription);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void RemoveSubscriptions(SubscriptionId subscriptionId)
        {
            Tuple<Type, SubscriptionId>[] subscriptionsSharingTheGivenId;
            Log.Debug("Acquired lock to ActiveSubscriptions at method RemoveSubscriptions(...)");
            lock (ActiveSubscriptions)
            {
                subscriptionsSharingTheGivenId = ActiveSubscriptions.Where(s => s.Key.Item2 == subscriptionId).Select(s => s.Key).ToArray();
            }
            Log.Debug("Released lock to ActiveSubscriptions at method RemoveSubscriptions(...)");
            foreach (var subscription in subscriptionsSharingTheGivenId)
            {
                RemoveSubscription(subscription.Item1, subscription.Item2);
            }
        }

        private void RemoveSubscription(Subscription subscription)
        {
            if (subscription == null)
                return;

            Log.Debug("Acquired lock to ActiveSubscriptions at method RemoveSubscription(...)");
            lock (ActiveSubscriptions)
            {
                ActiveSubscriptions.Remove(new Tuple<Type, SubscriptionId>(subscription.MessageType, subscription.SubscriptionId));
            }
            Log.Debug("Released lock to ActiveSubscriptions at method RemoveSubscription(...)");

            if (subscription.SubscriptionMode == SubscriptionMode.Exclusive)
                DeleteSubscriptionQueue(subscription.MessageType, subscription.SubscriptionId);

            subscription.Dispose();
        }

        public void DeleteSubscriptionQueue(Type messageType, SubscriptionId subscriptionId)
        {
            var queueName = QueueNameFor(messageType, subscriptionId);
            if (QueueExists(queueName))
                DeleteQueue(queueName);
        }

        [Log]
        [LogException]
        public void TrySubscribeTo(Type messageType, SubscriptionId subscriptionId, TopicId topicId, Action<object> messageHandler, SubscriptionMode subscriptionMode)
        {
            if (subscriptionId == null) throw new ArgumentNullException("subscriptionId");
            if (topicId == null) throw new ArgumentNullException("topicId");
            if (messageHandler == null) throw new ArgumentNullException("messageHandler");

            var subscription = SubscriptionFor(messageType, subscriptionId, topicId, messageHandler, subscriptionMode);
            subscription.Start();
        }

        private InSingleProcessMemorySubscription SubscriptionFor(Type messageType, SubscriptionId subscriptionId, TopicId topicId, Action<object> messageHandler, SubscriptionMode subscriptionMode)
        {
            var subscription = ExistingSubscriptionFor(messageType, subscriptionId);
            if ((subscription != null) && (subscription.MessageHandler != null)) //MessageHanlder == null indicates the subscription was prepared, but not yet performed
                throw new InvalidOperationException(String.Format("There is already a subscription for message type '{0}' and subscription id {1}", messageType, subscriptionId));

            subscription = NewSubscriptionFor(messageType, subscriptionId, topicId, subscriptionMode, !IgnoreMessagesOlderThanSubscriptionTime, messageHandler);

            return subscription;
        }

        private InSingleProcessMemorySubscription ExistingSubscriptionFor(Type messageType, SubscriptionId subscriptionId)
        {
            Log.Debug("Acquired lock to ActiveSubscriptions at method ExistingSubscriptionFor(...)");
            try
            {
                lock (ActiveSubscriptions)
                {
                    InSingleProcessMemorySubscription subscription;
                    ActiveSubscriptions.TryGetValue(new Tuple<Type, SubscriptionId>(messageType, subscriptionId), out subscription);
                    return subscription;
                }
            }
            finally
            {
                Log.Debug("Released lock to ActiveSubscriptions at method ExistingSubscriptionFor(...)");
            }
        }

        private InSingleProcessMemorySubscription NewSubscriptionFor(Type messageType, SubscriptionId subscriptionId, TopicId topicId, SubscriptionMode subscriptionMode, bool acceptMessagesOlderThanSubscriptionTime, Action<object> messageHandler)
        {
            var queueName = QueueNameFor(messageType, subscriptionId);
            var subscription = NewSubscribeSubscription(messageType, subscriptionId, topicId, subscriptionMode, acceptMessagesOlderThanSubscriptionTime, messageHandler, queueName);

            if (IsDisposing)
                throw new InvalidOperationException("Cannot create subscriptions after the SubscriptionBus be disposed!");

            Log.Debug("Acquired lock to ActiveSubscriptions at method NewSubscriptionFor(...)");
            lock (ActiveSubscriptions)
            {
                ActiveSubscriptions[new Tuple<Type, SubscriptionId>(messageType, subscriptionId)] = subscription;
            }
            Log.Debug("Released lock to ActiveSubscriptions at method NewSubscriptionFor(...)");

            return subscription;
        }

        protected virtual InSingleProcessMemorySubscription NewSubscribeSubscription(Type messageType, SubscriptionId subscriptionId, TopicId topicId, SubscriptionMode subscriptionMode,
            bool acceptMessagesOlderThanSubscriptionTime, Action<object> messageHandler, string queueName)
        {
            var subscription = new InSingleProcessMemorySubscription(
                this, subscriptionId, topicId, messageType, queueName, subscriptionMode, acceptMessagesOlderThanSubscriptionTime,
                messageHandler, null, null, null);
            return subscription;
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void PrepareSubscriptionTo<TMessage>(SubscriptionId subscriptionId) where TMessage : class, new()
        {
            PrepareSubscriptionTo(typeof(TMessage), TopicId.Default, subscriptionId);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void PrepareSubscriptionTo(Type messageType, SubscriptionId subscriptionId)
        {
            PrepareSubscriptionTo(messageType, TopicId.Default, subscriptionId);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void PrepareSubscriptionTo<TMessage>(TopicId topicId, SubscriptionId subscriptionId) where TMessage : class, new()
        {
            PrepareSubscriptionTo(typeof(TMessage), topicId, subscriptionId);
        }

        public void PrepareSubscriptionTo(Type messageType, TopicId topicId, SubscriptionId subscriptionId)
        {
            var subscription = NewSubscriptionFor(messageType, subscriptionId, topicId, SubscriptionMode.Shared, true, null);
            using (var model = NewChannel())
            {
                PrepareSubscription(model, subscription);
            }
        }

        public IEnumerable<SubscriptionId> AvailableSubscriptionsFor<TMessage>()
        {
            using (var model = NewChannel())
            {
                var queueName = QueueNameFor(typeof(TMessage));
                return model.QueueList().Where(q => q != queueName && q.StartsWith(queueName)).Select(q =>
                {
                    var subscriptionId = q.Substring(queueName.Length + 1);
                    return SubscriptionId.FromString(subscriptionId);
                });
            }
        }

        internal static void PrepareSubscription(InSingleProcessMemoryChannel model, Subscription subscription)
        {
            if (subscription.TopicId == TopicId.None)
            {
                DeclareQueue(model, subscription.QueueName, subscription.SubscriptionMode == SubscriptionMode.Exclusive);
            }
            else
            {
                DeclareQueue(model, subscription);
                DeclareExchange(model, subscription);
                BindQueue(model, subscription);
            }
            Log.Info("Prepared subscription for queue '{0}'", subscription.QueueName);
        }

        private bool IsDisposing;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposing = true;

                Log.Debug("Acquired lock to ActiveSubscriptions at method Dispose(...)");
                lock (ActiveSubscriptions)
                {
                    while (ActiveSubscriptions.Count > 0)
                    {
                        RemoveSubscription(ActiveSubscriptions.First().Value);
                    }
                }
                Log.Debug("Released lock to ActiveSubscriptions at method Dispose(...)");
            }
        }
    }
}
