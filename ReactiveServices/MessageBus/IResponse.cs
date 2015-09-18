namespace ReactiveServices.MessageBus
{
    public interface IResponse
    {
        ResponderId ResponderId { get; }
        ResponseId ResponseId { get; }
        IRequest Request { get; set; }
    }
}
