using System;

namespace ReactiveServices.MessageBus
{
    [Serializable]
    public class CannotDeleteSubscriptionQueueForExistingSubscriptionException : Exception
    {
        public CannotDeleteSubscriptionQueueForExistingSubscriptionException()
            : base("Cannot delete subscription queue for existing subscription!")
        {
        }
    }
}
