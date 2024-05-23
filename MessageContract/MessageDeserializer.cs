using Newtonsoft.Json.Linq;

namespace MessageContract.Tests;

public static class MessageDeserializer
{
    public static Dictionary<string, TMessage> DeserializeFilesContent<TMessage>(Dictionary<string, string> messageTemplateDict)
    {
        var deserializedMessageTemplate = new Dictionary<string, TMessage>();

        foreach (var kvp in messageTemplateDict)
        {
            JObject messageTemplateDictValue = JObject.Parse(kvp.Value);
            string eventType = messageTemplateDictValue["type"].ToString();
            string eventData = messageTemplateDictValue["data"].ToString();
            Type messageType = EventTypeMapper.GetTypeForEventName(eventType);

            if (messageType.ToString().Contains(typeof(TMessage).Name))
            {
                var deserializedEvent = DeserializeEvent(messageType, eventData);
                deserializedMessageTemplate.Add(kvp.Key, deserializedEvent);
            }
        }
        return deserializedMessageTemplate;
    }

    public static dynamic DeserializeEvent(Type messageType, string eventData)
    {
        JObject messageDataObj = JObject.Parse(eventData); // Parse eventData to JObject
        return messageDataObj.ToObject(messageType);
    }
}

