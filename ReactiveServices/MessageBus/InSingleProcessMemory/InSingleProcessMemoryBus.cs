using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using ReactiveServices.Extensions;

namespace ReactiveServices.MessageBus.InSingleProcessMemory
{
    public static class InSingleProcessMemoryBus
    {
        public class InSingleProcessMemoryMessage
        {
            public InSingleProcessMemoryMessage(ulong deliveryTag, string routingKey, IBasicProperties properties,
                object body)
            {
                DeliveryTag = deliveryTag;
                RoutingKey = routingKey;
                BasicProperties = properties;
                Body = body;
            }

            public ulong DeliveryTag { get; private set; }
            public string RoutingKey { get; private set; }
            public IBasicProperties BasicProperties { get; private set; }
            public object Body { get; private set; }
        }

        public struct InSingleProcessMemoryBinding
        {
            public string QueueName;
            public string ExchangeName;
            public string RoutingKey; // # means everything allowed, otherwise must match a topicid or queuename
        }

        static InSingleProcessMemoryBus()
        {
            BasicAcks += delegate { };
            BasicNacks += delegate { };
            StartDispatchingIncomingMessages();
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<string, Queue<InSingleProcessMemoryMessage>> Queues = new Dictionary<string, Queue<InSingleProcessMemoryMessage>>();
        private static readonly Dictionary<string, Queue<InSingleProcessMemoryMessage>> Exchanges = new Dictionary<string, Queue<InSingleProcessMemoryMessage>>();
        private static readonly List<InSingleProcessMemoryBinding> Bindings = new List<InSingleProcessMemoryBinding>();

        private static bool ExchangesModified;

        private static void StartDispatchingIncomingMessages()
        {
            Task.Factory.StartNew(() =>
            {
                string[] exchanges = null;
                while (true)
                {
                    if (exchanges == null || ExchangesModified)
                    {
                        ExchangesModified = false;

                        lock (Exchanges)
                            exchanges = Exchanges.Keys.ToArray();
                    }

                    //Thread.Sleep(1);

                    foreach (var exchange in exchanges)
                    {
                        DispatchMessagesToBoundQueues(exchange);
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        private static void DispatchMessagesToBoundQueues(string exchange)
        {
            Queue<InSingleProcessMemoryMessage> exchangeBag;
            lock (Exchanges)
                Exchanges.TryGetValue(exchange, out exchangeBag);

            if (exchangeBag != null)
            {
                while (exchangeBag.Count > 0)
                {
                    InSingleProcessMemoryMessage message;
                    lock (exchangeBag)
                        message = exchangeBag.Dequeue();

                    DispatchMessageToBoundQueues(exchange, message);
                }
            }
        }

        private static void DispatchMessageToBoundQueues(string exchange, InSingleProcessMemoryMessage message)
        {
            InSingleProcessMemoryBinding[] bindings;
            lock (Bindings)
                bindings = Bindings.ToArray();

            foreach (var binding in bindings)
            {
                if (IsBoundExchange(exchange, binding) && (IsBoundRoutingKey(message, binding)))
                {
                    Queue<InSingleProcessMemoryMessage> queueBag;
                    lock (Queues)
                        Queues.TryGetValue(binding.QueueName, out queueBag);

                    if (queueBag != null)
                    {
                        lock (queueBag)
                            queueBag.Enqueue(message);
                    }
                }
            }

            BasicAcks(typeof(InSingleProcessMemoryBus), new BasicAckEventArgs(message.DeliveryTag));
        }

        private static bool IsBoundRoutingKey(InSingleProcessMemoryMessage message, InSingleProcessMemoryBinding binding)
        {
            return binding.RoutingKey == message.RoutingKey || binding.RoutingKey == "#";
        }

        private static bool IsBoundExchange(string exchange, InSingleProcessMemoryBinding binding)
        {
            return binding.ExchangeName == exchange;
        }

        public static ulong NextPublishSeqNo;

        internal static uint QueueDelete(string queue)
        {
            lock (Queues)
                Queues.Remove(queue);

            return 0;
        }

        internal static QueueDeclareOk QueueDeclare(string queue, bool exclusive)
        {
            //exclusive parameter is ignored

            lock (Queues)
                if (!Queues.ContainsKey(queue))
                    Queues[queue] = new Queue<InSingleProcessMemoryMessage>();

            EnsureQueueIsBoundToAMQPDefault(queue);

            return QueueDeclareOk.Ok;
        }

        private static void EnsureQueueIsBoundToAMQPDefault(string queue)
        {
            //The default exchange is implicitly bound to every queue, with a routing key equal to the queue name
            QueueBind(queue, String.Empty, queue);
        }

        internal static void QueueBind(string queue, string exchange, string routingKey)
        {
            var binding = new InSingleProcessMemoryBinding
            {
                ExchangeName = exchange,
                QueueName = queue,
                RoutingKey = routingKey
            };
            lock (Bindings)
                if (!Bindings.Contains(binding))
                    Bindings.Add(binding);
        }

        internal static void ExchangeDelete(string exchange)
        {
            lock (Bindings)
                Bindings.RemoveAll(b => b.ExchangeName == exchange);
            lock (Exchanges)
            {
                Exchanges.Remove(exchange);
                ExchangesModified = true;
            }
        }

        internal static void ExchangeDeclare(string exchange, string type)
        {
            // type parameter is ignored
            lock (Exchanges)
                if (!Exchanges.ContainsKey(exchange))
                {
                    Exchanges[exchange] = new Queue<InSingleProcessMemoryMessage>();
                    ExchangesModified = true;
                }
        }

        internal static void BasicPublish(string exchange, string routingKey, IBasicProperties basicProperties, object body)
        {
            EnsureAMQPDefaultExists();

            Queue<InSingleProcessMemoryMessage> exchangeBag;
            lock (Exchanges)
                Exchanges.TryGetValue(exchange, out exchangeBag);

            if (exchangeBag == null)
                throw new ArgumentException("Could not find exchange {0}", exchange);

            var message = new InSingleProcessMemoryMessage
            (
                NextPublishSeqNo++,
                routingKey,
                basicProperties,
                body
            );

            lock (exchangeBag)
            {
                exchangeBag.Enqueue(message);
            }
        }

        private static void EnsureAMQPDefaultExists()
        {
            ExchangeDeclare(String.Empty, "topic");
        }

        internal static void BasicConsume(string queue, bool noAck, IBasicConsumer consumer)
        {
            // noAck is ignored
            consumer.StartConsumingThread(() =>
            {
                Log.Info("Starting consuming thread at consumer '{0}'", consumer.Id);
                while (true)
                {
                    if (consumer.IsCancellationRequested)
                        break;

                    Queue<InSingleProcessMemoryMessage> queueBag;
                    lock (Queues)
                        Queues.TryGetValue(queue, out queueBag);

                    //Thread.Sleep(1);

                    if (queueBag != null)
                        DequeueAndHandleNextMessage(consumer, queueBag);
                }
            });
        }

        private static void DequeueAndHandleNextMessage(IBasicConsumer consumer, Queue<InSingleProcessMemoryMessage> queueBag)
        {
            InSingleProcessMemoryMessage message = null;
            lock (queueBag)
                if (queueBag.Count != 0)
                    message = queueBag.Dequeue();

            if (message == null)
                return;

            // Discard expired messages
            var timestamp = message.BasicProperties.Timestamp;
            if (timestamp > 0 && !String.IsNullOrWhiteSpace(message.BasicProperties.Expiration))
            {
                var expiration = Int64.Parse(message.BasicProperties.Expiration);
                var now = DateTime.Now.ToUnixTime();
                if ((now - timestamp) > expiration)
                    return;
            }

            consumer.HandleBasicDeliver(message.DeliveryTag, message.BasicProperties, message.Body);

            // if do not receive an ack after handle deliver, requeue the message
            if (!AcksReceivedFromConsumers.ContainsKey(message.DeliveryTag))
            {
                lock (queueBag)
                    queueBag.Enqueue(message);
            }
        }

        private static readonly Dictionary<ulong, bool> AcksReceivedFromConsumers = new Dictionary<ulong, bool>();

        /// <summary>
        /// Publish an ack after the message is handled by a consumer
        /// </summary>
        public static void BasicAck(ulong deliveryTag, bool multiple)
        {
            // multiple parameter is ignored
            AcksReceivedFromConsumers[deliveryTag] = true;
        }

        /// <summary>
        /// Raised when ack is received after publishing
        /// </summary>
        public static event EventHandler<BasicAckEventArgs> BasicAcks;
        /// <summary>
        /// Raised when nack is received after publishing
        /// </summary>
        public static event EventHandler<BasicNackEventArgs> BasicNacks;

        internal static IEnumerable<string> QueueList()
        {
            lock (Queues)
                return Queues.Keys;
        }

        internal static bool ExchangeExists(string exchangeName)
        {
            lock (Exchanges)
                return Exchanges.ContainsKey(exchangeName);
        }

        internal static bool QueueExists(string queueName)
        {
            lock (Queues)
                return Queues.ContainsKey(queueName);
        }

        internal static void SimulateRestart()
        {
            //Restart simulation delete non-persistent messages on queues
            var queues = Queues.Keys.ToArray();
            foreach (var queue in queues)
            {
                Queue<InSingleProcessMemoryMessage> queueBag = null;
                lock (Queues)
                    Queues.TryGetValue(queue, out queueBag);
                if (queueBag != null)
                {
                    queueBag =
                        new Queue<InSingleProcessMemoryMessage>(
                            queueBag.Where(q => q.BasicProperties.DeliveryMode == (byte)2));
                    lock (Queues)
                        Queues[queue] = queueBag;
                }
            }
        }
    }
}
