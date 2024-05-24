using FluentAssertions;
using Newtonsoft.Json.Linq;
using PactNet;
using PactNet.Matchers;
using PactNet.Output.Xunit;
using SimCorp.Gain.Messages.Shared.Workflow;
using SimCorp.Gain.Messages.System.Workflow;
using System.Text.Json;
using Xunit.Abstractions;

namespace MessageContract.Tests;

public class StockEventProcessorTests
{
    private readonly IMessagePactBuilderV4 _messagePact;

    public StockEventProcessorTests(ITestOutputHelper output)
    {
        IPactV4 v4 = Pact.V4("Stock Event Consumer", "Stock Event Producer", new PactConfig
        {
            PactDir = "../../../../pacts/",
            DefaultJsonSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            },
            Outputters = new[]
          {
        new XunitOutput(output)
      }
        });
        _messagePact = v4.WithMessageInteractions();
    }

    [Fact]
    public void ReceiveSomeStockEvents()
    {
        string filePath = Path.Combine("MessageContract", "MessageContract", "MessageFromGain");
        List<JObject> messages = ReadActualPublishedMessage(filePath);
        List<JObject> groupbyMessages = GroupByMessageWithType(messages);
        List<JObject> firstMessageOfEachType = GetFirstMessageOfEachType(groupbyMessages);

        this._messagePact
          .ExpectsToReceive("some stock ticker events")
          .Given("A list of events is pushed to the queue")
          .WithMetadata("key", "valueKey")
            .WithJsonContent(Match.MinType(new
            {
                //Expectations on each property of one message type
                StartArg = Match.Type("sss"),
                Description = Match.Type("dff"),
                Priority = ProcessPriority.Highest,
                Ancestry = Match.Null(),
                StartsBulkOperation = Match.Type(false),
                Args = Match.Null(),
                ContextData = Match.Null(),
                WorkflowId = Match.Integer(0),
                WorkflowType = Match.Type(WorkflowType.Business),
                CorrelationId = Match.Null(),
                BulkCorrelationId = Match.Type("xxx"),
                Alarms = Match.Null(),
                WorkflowCorrelation = Match.Null()
            }, 1))
          .Verify<ICollection<WorkflowCreated>>(events =>
          {
              events.Should().BeEquivalentTo(new[]
          {

                // List of expected events, read from MessageFromGain.json (deserialized to WorkflowCreated type)
                // Each _messagePact only validates one type of message
                // MessageFromGain.json will be updated by the consumer manually when the expectations change
                // or when the provider provides a new version
              new WorkflowCreated
            {
                StartArg = "sss",
                Description = "dff",
                Priority = ProcessPriority.Highest,
                StartsBulkOperation=false
            }
            }); ;
          });
        //MessageFromGain.json with real actual value will be feed to Consumer in another tests to check if it can handle it correctly 
    }

    private List<JObject> ReadActualPublishedMessage(string filePath)
    {
        var messages = new List<JObject>();

        foreach (var file in Directory.GetFiles(filePath, "*.json"))
        {
            string content = File.ReadAllText(file);
            messages.Add(JObject.Parse(content));
        }

        return messages;
    }

    private List<JObject> GroupByMessageWithType(List<JObject> messages)
    {
        // Implementation to group messages by their type
        var groupedMessages = messages
            .GroupBy(msg => (string)msg["type"])
            .Select(group => new JObject(new JProperty("type", group.Key), new JProperty("messages", new JArray(group))))
            .ToList();

        return groupedMessages;
    }

    private List<JObject> GetFirstMessageOfEachType(List<JObject> groupedMessages)
    {
        // Implementation to get the first message of each type
        var firstMessages = new List<JObject>();

        foreach (var group in groupedMessages)
        {
            var messages = (JArray)group["messages"];
            if (messages.Count > 0)
            {
                firstMessages.Add((JObject)messages[0]);
            }
        }

        return firstMessages;
    }
}
}