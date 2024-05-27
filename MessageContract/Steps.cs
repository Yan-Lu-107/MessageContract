using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MessageContract.Tests
{
    public static class Steps
    {
        public static Dictionary<string, List<object>> PrepareData(string filePath)
        {
            List<JObject> actualMessages = ReadMessageList(filePath);
            return GroupByMessageWithType(actualMessages);

        }

        private static List<JObject> ReadMessageList(string filePath)
        {
            string jsonContent = File.ReadAllText(Path.Combine(filePath));
            List<JObject> jsonObjects = new List<JObject>();

            using (JsonTextReader reader = new JsonTextReader(new StringReader(jsonContent)))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        // Load JObject from the reader
                        JObject jsonObject = JObject.Load(reader);
                        jsonObjects.Add(jsonObject);
                    }
                }
            }

            return jsonObjects;
        }

        private static Dictionary<string, List<object>> GroupByMessageWithType(List<JObject> messages)
        {
            // Implementation to group messages by their type
            //var groupedMessages = messages
            //    .GroupBy(msg => (string)msg["type"])
            //    .Select(group => new JObject(new JProperty("type", group.Key), new JProperty("messages", new JArray(group))))
            //    .ToList();

            var expectedGroups = new Dictionary<string, List<object>>();
            foreach (JObject message in messages)
            {
                var typeName = message["type"].ToString();

                if (expectedGroups.ContainsKey(typeName))
                {
                    expectedGroups[typeName].Add(DeserializeCloudEventMessage(message));
                }
                else
                {
                    expectedGroups.Add(typeName, new List<object>() { DeserializeCloudEventMessage(message) });
                }
            }

            return expectedGroups;
        }

        public static object DeserializeCloudEventMessage(JObject messageObj)
        {
            string eventType = messageObj["type"].ToString();
            string eventData = messageObj["data"].ToString();
            Type messageType = EventTypeMapper.GetTypeForEventName(eventType);
            JObject messageDataObj = JObject.Parse(eventData);

            return messageDataObj.ToObject(messageType);
        }
    }

}