
using System;

namespace ReactiveServices.MessageBus
{
    public class SubscriptionId : Id<SubscriptionId>, IHasCreationDate
    {
        public SubscriptionId()
        {
            CreationDate = DateTime.Now;
        }

        public DateTime CreationDate { get; private set; }
        
        public bool IsNewerThan(IHasCreationDate reference)
        {
            return CreationDate > reference.CreationDate;
        }
    }
}
