using NLog;
using System;
using System.Globalization;
using ReactiveServices.Extensions;

namespace ReactiveServices.MessageBus.InSingleProcessMemory
{
    public interface IInSingleProcessMemoryMessageBus
    {
        InSingleProcessMemoryChannel NewChannel();
        bool QueueExists(string queueName);
    }

    public abstract class InSingleProcessMemoryMessageBus : IMessageBus, IInSingleProcessMemoryMessageBus
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();


        public InSingleProcessMemoryChannel NewChannel()
        {
            var model = new InSingleProcessMemoryChannel();
            return model;
        }

        protected static string QueueNameFor(Type messageType)
        {
            //Ex: ReactiveServices.ComputationalUnit.Dispatching.LaunchConfirmation:ComputationalUnit.Dispatching
            //Pattern: FullTypeName:AssemblyName_SubscriptionId
            return String.Format("{0}:{1}", messageType.FullName, messageType.Assembly.GetName().Name);
        }

        protected static string QueueNameFor(Type messageType, SubscriptionId subscriptionId)
        {
            //Ex: ReactiveServices.ComputationalUnit.Dispatching.LaunchConfirmation:ComputationalUnit.Dispatching_LaunchConfirmationSubscriptionFor_DispatcherLauncherId#c73c2193-a8c5-47b6-ac5a-cde91a4c788b‏
            //Pattern: FullTypeName:AssemblyName_SubscriptionId
            return String.Format("{0}:{1}_{2}", messageType.FullName, messageType.Assembly.GetName().Name, subscriptionId.Value);
        }

        protected static string ExchangeNameFor(Type messageType)
        {
            //Ex: ReactiveServices.ComputationalUnit.Dispatching.LaunchConfirmation:ComputationalUnit.Dispatching‏
            //Pattern: FullTypeName:AssemblyName
            return String.Format("{0}:{1}", messageType.FullName, messageType.Assembly.GetName().Name);
        }

        protected static void BindQueue(InSingleProcessMemoryChannel model, Subscription subscription)
        {
            var queueName = QueueNameFor(subscription.MessageType, subscription.SubscriptionId);
            var exchangeName = ExchangeNameFor(subscription.MessageType);
            var routingKey = RoutingKeyFor(subscription.TopicId);
            model.QueueBind(queueName, exchangeName, routingKey);
            Log.Info("Queue '{0}' bound to exchange {1}!", queueName, exchangeName);
        }

        protected static string RoutingKeyFor(TopicId topicId)
        {
            var routingKey = topicId == TopicId.Default ? "#" : topicId.Value;
            return routingKey;
        }

        protected bool ExchangeExists(string exchangeName)
        {
            using (var model = NewChannel())
            {
                return model.ExchangeExists(exchangeName);
            }
        }

        protected void DeleteExchange(string exchangeName)
        {
            using (var model = NewChannel())
            {
                model.ExchangeDelete(exchangeName);
            }
        }

        protected static void DeclareExchange(InSingleProcessMemoryChannel model, Subscription subscription)
        {
            var exchangeName = ExchangeNameFor(subscription.MessageType);
            model.ExchangeDeclare(exchangeName, "topic");
            Log.Info("Exchange '{0}' declared!", exchangeName);
        }

        protected static void DeclareQueue(InSingleProcessMemoryChannel model, Subscription subscription)
        {
            var queueName = QueueNameFor(subscription.MessageType, subscription.SubscriptionId);
            var isExclusive = subscription.SubscriptionMode == SubscriptionMode.Exclusive;
            DeclareQueue(model, queueName, isExclusive);
        }

        internal static void DeclareQueue(InSingleProcessMemoryChannel model, string queueName, bool isExclusive)
        {
            model.QueueDeclare(queueName, isExclusive);
            Log.Info("Queue '{0}' declared!", queueName);
        }

        protected void DeclareQueue(string queueName)
        {
            using (var model = NewChannel())
            {
                DeclareQueue(model, queueName, false);
            }
        }

        public bool QueueExists(string queueName)
        {
            using (var model = NewChannel())
            {
                return model.QueueExists(queueName);
            }
        }

        protected void DeleteQueue(string queueName)
        {
            using (var model = NewChannel())
            {
                model.QueueDelete(queueName);
            }
        }

        protected void SetMessageExpirationTimespan(IBasicProperties props, TimeSpan expiration)
        {
            var timestamp = DateTime.Now.ToUnixTime();
            props.Timestamp = timestamp;
            if (expiration != default(TimeSpan))
                props.Expiration = expiration.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
            }
            // Free native resources

            //Log.Error("Disposing {0}", GetType().Name);
        }
    }
}
