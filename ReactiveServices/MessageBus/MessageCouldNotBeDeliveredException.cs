using System;

namespace ReactiveServices.MessageBus
{
    [Serializable]
    public class NoPublishConfirmationResponseForPublishedMessageException : Exception
    {
        public NoPublishConfirmationResponseForPublishedMessageException()
            : base("Message could not be delivered. No response from messaging middleware!") 
        {
        }

        public NoPublishConfirmationResponseForPublishedMessageException(string messageType)
            : base(String.Format("Message of type {0} could not be delivered!. No response from messaging middleware", messageType))
        {
        }
    }

    [Serializable]
    public class NackReceivedAsPublishConfirmationResponseForPublishedMessageException : Exception
    {
        public NackReceivedAsPublishConfirmationResponseForPublishedMessageException()
            : base("Message could not be delivered!. Messaging middleware responded with a NACK")
        {
        }

        public NackReceivedAsPublishConfirmationResponseForPublishedMessageException(string messageType)
            : base(String.Format("Message of type {0} could not be delivered!. Messaging middleware responded with a NACK", messageType))
        {
        }
    }
}
