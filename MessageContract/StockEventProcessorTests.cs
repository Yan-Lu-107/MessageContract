using FluentAssertions;
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
        this._messagePact
          .ExpectsToReceive("some stock ticker events")
          .Given("A list of events is pushed to the queue")
          .WithMetadata("key", "valueKey")
            .WithJsonContent(Match.MinType(new
            {
                StartArg = Match.Type("sss"),
                Description = Match.Type("dff"),
                Priority = Match.Type(ProcessPriority.Highest),
                Ancestry = Match.Null(),
                StartsBulkOperation = Match.Type(false),
                Args = Match.Null(),
                ContextData = Match.Null(),
                WorkflowId = Match.Integer(0),
                WorkflowType = Match.Type(WorkflowType.Business),
                CorrelationId = Match.Null(),
                BulkCorrelationId = Match.Null(),
                Alarms = Match.Null(),
                WorkflowCorrelation = Match.Null()
            }, 1))
          .Verify<ICollection<WorkflowCreated>>(events =>
          {
              events.Should().BeEquivalentTo(new[]
          {
              new WorkflowCreated
            {
                StartArg = "sss",
                Description = "dff",
                Priority = ProcessPriority.Highest
            }
            });
          });
    }
}