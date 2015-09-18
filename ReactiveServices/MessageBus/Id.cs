using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ReactiveServices.MessageBus
{
    [DataContract]
    [JsonObject]
    [JsonConverter(typeof(IdConverter))]
    public partial class Id
    {
        [DataMember]
        public string Value { get; set; }
        
        public virtual string SerializedValue
        {
            get
            {
                return Value;
            }
        }

        public override string ToString()
        {
            return GetType().Name + "#" + Value;
        }

        // Do not implement automatic convertions from string. 
        // The purpose of a specialized Id is to avoid the id to be confused 
        // by some other string, so an automatic conversion should not exist
        //public static implicit operator Id(string anId)
        //{
        //    return Id.FromString(anId);
        //}
    }

    [DataContract]
    public class Id<T> : Id
        where T : Id<T>, new()
    {
        [DebuggerStepThrough]
        public static T FromString(string idValue)
        {
            var newId = new T { Value = idValue };
            return newId;
        }

        [DebuggerStepThrough]
        public static T FromGuid(Guid idValue)
        {
            var newId = new T { Value = idValue.ToString() };
            return newId;
        }

        private static T _undefined;

        public static T Undefined
        {
            get { return _undefined ?? (_undefined = FromString("Undefined")); }
        }

        [DebuggerStepThrough]
        public static T New()
        {
            return FromGuid(Guid.NewGuid());
        }
    }
}
