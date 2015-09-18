using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;

namespace ReactiveServices.MessageBus.InSingleProcessMemory
{
    /// <summary>
    /// Decorator over InSingleProcessMemory.Client.IModel that handles its own exclusive Connection
    /// </summary>
    [Log(AttributeExclude = true)]
    [LogException(AttributeExclude = true)]
    public sealed class InSingleProcessMemoryChannel : IDisposable
    {
        public void BasicAck(ulong deliveryTag, bool multiple)
        {
            InSingleProcessMemoryBus.BasicAck(deliveryTag, multiple);
        }

        public event EventHandler<BasicAckEventArgs> BasicAcks
        {
            add { InSingleProcessMemoryBus.BasicAcks += value; }
            remove { InSingleProcessMemoryBus.BasicAcks -= value; }
        }
        public event EventHandler<BasicNackEventArgs> BasicNacks
        {
            add { InSingleProcessMemoryBus.BasicNacks += value; }
            remove { InSingleProcessMemoryBus.BasicNacks -= value; }
        }

        public void BasicConsume(string queue, bool noAck, IBasicConsumer consumer)
        {
            InSingleProcessMemoryBus.BasicConsume(queue, noAck, consumer);
        }

        public void BasicPublish(string exchange, string routingKey, IBasicProperties basicProperties, object body)
        {
            InSingleProcessMemoryBus.BasicPublish(exchange, routingKey, basicProperties, body);
        }

        public void BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
        }

        public void Close()
        {
        }

        public void ConfirmSelect()
        {
        }

        public IBasicProperties CreateBasicProperties()
        {
            var properties = new BasicProperties
            {
                Headers = new Dictionary<string, object>()
            };
            return properties;
        }

        public void ExchangeDeclare(string exchange, string type)
        {
            InSingleProcessMemoryBus.ExchangeDeclare(exchange, type);
        }

        public void ExchangeDelete(string exchange)
        {
            InSingleProcessMemoryBus.ExchangeDelete(exchange);
        }

        public bool IsClosed
        {
            get { return false; }
        }

        public bool IsOpen
        {
            get { return true; }
        }

        public ulong NextPublishSeqNo
        {
            get { return InSingleProcessMemoryBus.NextPublishSeqNo; }
        }

        public void QueueBind(string queue, string exchange, string routingKey)
        {
            InSingleProcessMemoryBus.QueueBind(queue, exchange, routingKey);
        }

        public QueueDeclareOk QueueDeclare(string queue, bool exclusive)
        {
            return InSingleProcessMemoryBus.QueueDeclare(queue, exclusive);
        }

        public uint QueueDelete(string queue)
        {
            return InSingleProcessMemoryBus.QueueDelete(queue);
        }

        public void Dispose()
        {
        }

        public IEnumerable<string> QueueList()
        {
            return InSingleProcessMemoryBus.QueueList();
        }

        public bool ExchangeExists(string exchangeName)
        {
            return InSingleProcessMemoryBus.ExchangeExists(exchangeName);
        }

        public bool QueueExists(string queueName)
        {
            return InSingleProcessMemoryBus.QueueExists(queueName);
        }
    }
}
