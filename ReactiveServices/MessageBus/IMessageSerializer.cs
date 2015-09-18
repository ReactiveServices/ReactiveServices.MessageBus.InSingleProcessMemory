using System;

namespace ReactiveServices.MessageBus
{
    public interface IMessageSerializer
    {
        object Deserialize(byte[] messageBytes, Type messageType);
        byte[] Serialize(Type messageType, object message);
    }
}