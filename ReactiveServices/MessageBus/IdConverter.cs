using System;
using Newtonsoft.Json;

namespace ReactiveServices.MessageBus
{
    public class IdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((Id)value).Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = (Id)Activator.CreateInstance(objectType);
            result.Value = (string)reader.Value;
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(Id));
        }
    }
}
