namespace ReactiveServices.MessageBus
{
    public interface IAuthorizedPublishingBus : IPublishingBus { }

    public interface IAuthorizedSubscriptionBus : ISubscriptionBus { }

    public interface IAuthorizedRequestBus : IRequestBus { }

    public interface IAuthorizedResponseBus : IResponseBus { }

    public interface IAuthorizedSendingBus : ISendingBus { }

    public interface IAuthorizedReceivingBus : IReceivingBus { }
}
