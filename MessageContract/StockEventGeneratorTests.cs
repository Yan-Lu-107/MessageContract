using PactNet.Verifier;
using SimCorp.Gain.Messages.System.Workflow;
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
            //.Add("a single event", () => new WorkflowCreated
            //  {
            //      Name = "AAPL",
            //      Price = 1.23m
            //  })
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
            // List of actual published messages in MessageFromGain.json (Deserialize to WorkflowCreated type)
            // Before this, we need to integrate this verification with functional tests which generate the actual published messages
            // and store in MessageFromGain.json 
              new WorkflowCreated { StartArg = "sss", Description = "dff", Priority = ProcessPriority.Highest },
              new WorkflowCreated { StartArg = "sss", Description = "dff", Priority = ProcessPriority.Highest }
                });
              });
          }, defaultSettings)
              .WithFileSource(new FileInfo(pactPath))
              .Verify();
    }
}