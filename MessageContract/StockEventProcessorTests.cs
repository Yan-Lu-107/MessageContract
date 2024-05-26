using FluentAssertions;
using PactNet;
using PactNet.Matchers;
using PactNet.Output.Xunit;
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
        //WIP
        //List<JObject> groupByMessageWithType = PrepareData("MessageFromGain.json");

        Dictionary<string, List<object>> expectedGroupByMessagesWithType = Steps.PrepareData("ExpectedMessages.json");
        ICollection<string> messageTypes = expectedGroupByMessagesWithType.Keys;
        foreach (string messageType in messageTypes)
        {
            Type eventType = EventTypeMapper.GetTypeForEventName(messageType);

            if (!expectedGroupByMessagesWithType.TryGetValue(messageType, out List<object> expectedGroupedMsgs))
            {
                throw new Exception($"No matching expected message group found");
            }

            this._messagePact
            .ExpectsToReceive($"{eventType.Name} Message from Gain for the feed upload request")
            .Given("WorkflowCreated events are pushed to the queue")
            .WithMetadata("key", "valueKey")
            .WithJsonContent(Match.MinType(MessageExpectationProvider.GetContent(eventType), expectedGroupedMsgs.Count))
            .Verify<ICollection<WorkflowCreated>>(events =>
            {
                events.Should().BeEquivalentTo(new[] { expectedGroupedMsgs[0] });
            });
        }
    }
}

//Issue:
//    1. How to use match to validate actual value is one of the given expected
//    2. Need to manually create expected message in the consumer test
//    3. Not easy to debug why the provider test fails

