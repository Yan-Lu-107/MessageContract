using PactNet.Matchers;
using SimCorp.Gain.Messages.Shared.Workflow;
using SimCorp.Gain.Messages.System.Workflow;

namespace MessageContract.Tests
{
    internal static class MessageExpectationProvider
    {
        private readonly static object workflowCreatedContent = new
        {
            //Expectations on each property of each message type
            StartArg = Match.Type("Aim.Gain.EDM.IntegrationService.Server.Processes.StartArguments.GenericFeedRequestStartArgument"),
            Description = Match.Type("Processing MessageContainer received from connector Gain API: Excel-Party"),
            Priority = Match.Type(ProcessPriority.Normal),
            //Ancestry = Match.Type(Array.Empty<object>()),
            StartsBulkOperation = Match.Type(true),
            //Args = Match.Type(new Dictionary<string, object> { { "TriggeredBy", Match.Type(new JsonObject()) } }),
            ContextData = Match.Type(new Dictionary<string, object>()),
            WorkflowId = Match.Number(11),
            WorkflowType = Match.Type(WorkflowType.Communication),
            CorrelationId = Match.Type("sba"),
            BulkCorrelationId = Match.Null(),
            //Alarms = Match.Null(),
            BreaksBulkSuccession = Match.Type(false),
            WorkflowCorrelation = Match.Null()
        };

        private readonly static object workflowFinishedContent = new
        {
            State = Match.Type(WorkflowFinishedState.Finished),
            WorkflowId = Match.Number(123),
            WorkflowType = Match.Type(WorkflowType.Communication),
            CorrelationId = Match.Type("abc")
        };

        private readonly static Dictionary<Type, object> expectedEventDict = new Dictionary<Type, object>
        {
            { typeof(WorkflowCreated) , workflowCreatedContent  },
            { typeof(WorkflowFinished), workflowFinishedContent }
        };

        public static object GetContent(Type eventType)
        {
            if (expectedEventDict.TryGetValue(eventType, out object content))
            {
                return content;
            }
            else
            {
                throw new ArgumentException($"{eventType} is not a recognised type name. Please consider adding this to {nameof(EventTypeMapper)}.");
            }
        }
    }
}