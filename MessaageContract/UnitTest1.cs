using FluentAssertions;
using PactNet;
using PactNet.Matchers;
using PactNet.Output.Xunit;
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
      PactDir = "../pacts/",
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
        Name = Match.Type("AAPL"),
        Price = Match.Decimal(1.23m),
        Timestamp = Match.Type(new DateTime(2022, 2, 14, 13, 14, 15, 678))
      }, 1))
      .Verify<ICollection<StockEvent>>(events =>
      {
        events.Should().BeEquivalentTo(new[]
        {
          new StockEvent
          {
            Name = "AAPl",
            Price = 1.23m,
            Timestamp = new DateTime(2022,2, 14, 13, 14, 15, 678)
          }
        });
      });
  }
}

public class StockEvent
{
  public string Name { get; set; }
  public decimal Price { get; set; }
  public DateTime Timestamp { get; set; }
}