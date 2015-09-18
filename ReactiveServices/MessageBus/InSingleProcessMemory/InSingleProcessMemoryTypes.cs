using System;
using System.Collections.Generic;
using System.Threading;

namespace ReactiveServices.MessageBus.InSingleProcessMemory
{
    public interface IBasicProperties
    {
        Dictionary<string, object> Headers { get; set; }
        byte DeliveryMode { get; set; }

        string CorrelationId { get; set; }

        string ReplyTo { get; set; }

        long Timestamp { get; set; }

        string Expiration { get; set; }
    }

    public interface IBasicConsumer
    {
        void HandleBasicDeliver(ulong deliveryTag, IBasicProperties properties, object body);
        void StartConsumingThread(Action consumeAction);
        bool IsCancellationRequested { get; }
        Guid Id { get; }
    }

    class BasicProperties : IBasicProperties
    {
        public Dictionary<string, object> Headers { get; set; }

        public byte DeliveryMode { get; set; }

        public string CorrelationId { get; set; }

        public string ReplyTo { get; set; }

        public long Timestamp { get; set; }

        public string Expiration { get; set; }
    }

    public class BasicAckEventArgs
    {
        public BasicAckEventArgs(ulong deliveryTag)
        {
            DeliveryTag = deliveryTag;
        }
        public ulong DeliveryTag { get; set; }
    }

    public class BasicNackEventArgs
    {
        public BasicNackEventArgs(ulong deliveryTag)
        {
            DeliveryTag = deliveryTag;
        }
        public ulong DeliveryTag { get; set; }
    }

    public class ConsumerEventArgs
    {
    }

    public class QueueDeclareOk
    {
        public static readonly QueueDeclareOk Ok = new QueueDeclareOk();
    }
}
