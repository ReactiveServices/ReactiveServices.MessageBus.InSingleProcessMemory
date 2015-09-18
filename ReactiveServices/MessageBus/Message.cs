using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ReactiveServices.MessageBus
{
    [DataContract]
    public abstract class Message
    {
        protected internal Message(MessageId messageId)
        {
            MessageId = messageId;
            CreationDate = DateTime.Now;
            Headers = new Dictionary<string, string>();
        }

        protected internal Message()
            : this(MessageId.New())
        {
        }

        [DataMember]
        public MessageId MessageId { get; protected set; }

        [DataMember]
        [JsonConverter(typeof(DateTimeToUnixDateConverter))]
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public Dictionary<string, string> Headers { get; private set; }

        public bool IsNewerThan(IHasCreationDate reference)
        {
            return CreationDate > reference.CreationDate;
        }

        public override string ToString()
        {
            return MessageId.Value;
        }
    }

    [DataContract]
    public class Request : Message, IRequest
    {
        public Request(RequesterId requesterId, RequestId requestId)
        {
            RequestId = requestId;
            RequesterId = requesterId;
        }
        [DataMember]
        public RequestId RequestId { get; set; }
        [DataMember]
        public RequesterId RequesterId { get; set; }
    }

    [DataContract]
    public class Response : Message, IResponse
    {
        public Response(ResponderId responderId, ResponseId responseId, IRequest request)
        {
            Request = request;
            ResponseId = responseId;
            ResponderId = responderId;
        }

        [DataMember]
        public ResponseId ResponseId { get; set; }
        [DataMember]
        public ResponderId ResponderId { get; set; }
        [DataMember]
        public IRequest Request { get; set; }
    }
}
