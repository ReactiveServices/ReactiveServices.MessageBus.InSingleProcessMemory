namespace ReactiveServices.MessageBus
{
    public interface IRequest
    {
        RequestId RequestId { get; }
        RequesterId RequesterId { get; }
    }
}
