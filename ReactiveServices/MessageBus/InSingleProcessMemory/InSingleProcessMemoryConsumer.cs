using NLog;

using ReactiveServices.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveServices.MessageBus.InSingleProcessMemory
{
    public class InSingleProcessMemoryConsumer : IBasicConsumer, IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        internal InSingleProcessMemoryConsumer(InSingleProcessMemorySubscription subscription, IInSingleProcessMemoryMessageBus messageBus)
        {
            Subscription = subscription;
            MessageBus = messageBus;
            Model = MessageBus.NewChannel();
        }

        private readonly Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return _id; }
        }

        public InSingleProcessMemoryChannel Model { get; set; }

        private IInSingleProcessMemoryMessageBus MessageBus { get; set; }
        private Subscription Subscription { get; set; }

        public void Start()
        {
            //Must use the same model, otherwise the SubscriptionMode.Exclusive would not work
            InSingleProcessMemorySubscriptionBus.PrepareSubscription(Model, Subscription);

            Model.BasicQos(0, 1, false);
            Model.BasicConsume(Subscription.QueueName, false, this);

            WaitStartConsuming();

            Log.Info("Start consuming queue '{0}'", Subscription.QueueName);
        }

        private void WaitStartConsuming()
        {
            var timeout = TimeSpan.FromSeconds(1);
            var sw = new Stopwatch();
            sw.Start();
            while (!IsRunning)
            {
                if (sw.Elapsed > timeout)
                    break;

                Thread.Sleep(10);
            }
            sw.Stop();

            if (!IsRunning)
                throw new TimeoutException(String.Format("Could not subscribe to queue {0} within {1} second(s)!", Subscription.QueueName, timeout.TotalSeconds));
        }

        public bool IsRunning
        {
            get { return true; }
        }

        public void HandleBasicDeliver(ulong deliveryTag, IBasicProperties properties, object body)
        {
            if (!Model.IsOpen) return;

            Model.BasicAck(deliveryTag, false); // TODO: Review - Check case of failure on Message Handler

            Task.Run(() =>
            {
                var messageObject = body;

                var propertyHeaders = properties.Headers;
                var headers = new Dictionary<string, string>();
                if (propertyHeaders != null)
                {
                    foreach (var propertyHeader in propertyHeaders)
                    {
                        headers.Add(propertyHeader.Key, (string)propertyHeader.Value);
                    }
                }

                Log.Info("Executing handler for delivery tag '{0}' from queue '{1}'", deliveryTag, Subscription.QueueName);

                try
                {
                    ExecuteSubscriptionMessageHandler(properties, messageObject, headers);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Exception executing message handler for subscription '{0}'!", Subscription.SubscriptionId);
                    throw;
                }
            });
        }

        protected virtual void ExecuteSubscriptionMessageHandler(IBasicProperties properties, object messageObject, Dictionary<string, string> headers)
        {
            if (Subscription.MessageHandlerWithPropertiesAndHeaders != null)
                Subscription.MessageHandlerWithPropertiesAndHeaders(messageObject, properties, headers);

            if (Subscription.MessageHandlerWithProperties != null)
                Subscription.MessageHandlerWithProperties(messageObject, properties);

            if (Subscription.MessageHandlerWithHeaders != null)
                Subscription.MessageHandlerWithHeaders(messageObject, headers);

            if (Subscription.MessageHandler != null)
                Subscription.MessageHandler(messageObject);
        }

        public void Dispose()
        {
            Model.Close();
            Model.Dispose();

            ConsumingThreadCancellationTokenSource.Cancel();

            Log.Info("Stop consuming queue '{0}'", Subscription.QueueName);
        }

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;

        private readonly CancellationTokenSource ConsumingThreadCancellationTokenSource = new CancellationTokenSource();

        public void StartConsumingThread(Action consumeAction)
        {
            Task.Factory.StartNew(consumeAction, ConsumingThreadCancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public bool IsCancellationRequested
        {
            get
            {
                return ConsumingThreadCancellationTokenSource.IsCancellationRequested;
            }
        }
    }
}
