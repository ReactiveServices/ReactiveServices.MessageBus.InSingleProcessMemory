using System;

namespace ReactiveServices.MessageBus
{
    public interface IHasCreationDate
    {
        DateTime CreationDate { get; }
        bool IsNewerThan(IHasCreationDate reference);
    }
}
