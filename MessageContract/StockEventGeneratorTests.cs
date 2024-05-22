using PactNet.Verifier;
using System.Text.Json;

namespace MessageContract.Tests;

public class StockEventGeneratorTests : IDisposable
{
  private readonly PactVerifier _verifier;

  public StockEventGeneratorTests()
  {
    _verifier = new PactVerifier("Stock Event Producer");
  }

  public void Dispose()
  {
    // make sure you dispose the verifier to stop the internal messaging server
    GC.SuppressFinalize(this);
    _verifier.Dispose();
  }

  [Fact]
  public void EnsureEventApiHonorsPactWithConsumer()
  {
    string pactPath = Path.Combine("..", "..", "..", "..",
      "pacts",
      "Stock Event Consumer-Stock Event Producer.json");
    if (!File.Exists(pactPath))
    {
      throw new ArgumentException(pactPath);
    }

    var defaultSettings = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    ;
    _verifier
      .WithMessages(scenarios =>
      {
        // register the response to each interaction
        // the descriptions must match those in the pact file(s)
        scenarios
          .Add("a single event", () => new StockEvent
          {
            Name = "AAPL",
            Price = 1.23m
          })
          .Add("some stock ticker events", builder =>
            {
              builder
                .WithMetadata(new
                {
                  ContentType = "application/json",
                  Key = "valueKey"
                })
              .WithContent(() => new[]
              {
              new StockEvent { Name = "AAPL", Price = 1.23m },
              new StockEvent { Name = "TSLA", Price = 4.56m }
              });
            });
      }, defaultSettings)
          .WithFileSource(new FileInfo(pactPath))
          .Verify();
  }
}