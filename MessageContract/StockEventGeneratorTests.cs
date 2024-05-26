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
        Dictionary<string, List<object>> actualGroupByMessagesWithType = Steps.PrepareData("MessageTest.json");
        ICollection<string> messageTypes = actualGroupByMessagesWithType.Keys;
        foreach (string messageType in messageTypes)
        {
            Type eventType = EventTypeMapper.GetTypeForEventName(messageType);

            if (!actualGroupByMessagesWithType.TryGetValue(messageType, out List<object> actualGroupedMsgs))
            {
                throw new Exception($"No matching expected message group found");
            }
            _verifier
            .WithMessages(scenarios =>
            {
                scenarios
                .Add($"{eventType.Name} Message from Gain for the feed upload request", builder =>
                {
                    builder
                    .WithMetadata(new
                    {
                        ContentType = "application/json",
                        Key = "valueKey"
                    })
                .WithContent(() => actualGroupedMsgs);
                });
            }, defaultSettings)
            .WithFileSource(new FileInfo(pactPath))
            .Verify();
        }
    }
}