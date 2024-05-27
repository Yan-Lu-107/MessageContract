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
        Dictionary<string, List<object>> actualGroupByMessagesWithType = Steps.PrepareData("MessageTest.json");

        ICollection<string> messageTypes = expectedGroupByMessagesWithType.Keys;
        foreach (string messageType in messageTypes)
        {
            Type eventType = EventTypeMapper.GetTypeForEventName(messageType);
            int actualMessageCount = actualGroupByMessagesWithType.TryGetValue(messageType, out List<object>? actualGroupedMsgs)
                ? actualGroupedMsgs.Count
                : 1;

            if (!expectedGroupByMessagesWithType.TryGetValue(messageType, out List<object>? expectedGroupedMsgs))
            {
                throw new Exception($"No matching expected message group found");
            }

            if (eventType.Name == "WorkflowCreated")
            {
                this._messagePact
                 .ExpectsToReceive($"{eventType.Name} Message from Gain for the feed upload request")
                 .Given($"{eventType.Name} events are pushed to the queue")
                 .WithMetadata("key", "valueKey")
                 .WithJsonContent(Match.MinType(MessageExpectationProvider.GetContent(eventType), actualMessageCount))
                 .Verify<ICollection<WorkflowCreated>>(events =>
                 {
                     events.Should().BeEquivalentTo(new[] { expectedGroupedMsgs[0] });
                 });
            }

            else if (eventType.Name == "WorkflowFinished")
            {
                this._messagePact
                 .ExpectsToReceive($"{eventType.Name} Message from Gain for the feed upload request")
                 .Given($"{eventType.Name} events are pushed to the queue")
                 .WithMetadata("key", "valueKey")
                 .WithJsonContent(Match.MinType(MessageExpectationProvider.GetContent(eventType), actualMessageCount))
                 .Verify<ICollection<WorkflowFinished>>(events =>
                 {
                     events.Should().BeEquivalentTo(new[] { expectedGroupedMsgs[0] });
                 });
            }
            else if (eventType.Name == "WorkflowChangedBusinessObject")
            {
                this._messagePact
                 .ExpectsToReceive($"{eventType.Name} Message from Gain for the feed upload request")
                 .Given($"{eventType.Name} events are pushed to the queue")
                 .WithMetadata("key", "valueKey")
                 .WithJsonContent(Match.MinType(MessageExpectationProvider.GetContent(eventType), actualMessageCount))
                 .Verify<ICollection<WorkflowChangedBusinessObject>>(events =>
                 {
                     events.Should().BeEquivalentTo(new[] { expectedGroupedMsgs[0] });
                 });
            }
        }
    }
}


//Issue/Todo:
//How to use Match to validate that the actual value is one of the given expected values.
//Need to manually create the expected message in the consumer test (only for required fields):
//  Define expected values for some properties.
//  Skip non-required fields.
//Not easy to debug why the provider test fails.