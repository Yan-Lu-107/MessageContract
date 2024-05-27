using PactNet.Matchers;
using SimCorp.Gain.Messages.Shared.Workflow;
using SimCorp.Gain.Messages.System.Workflow;

namespace MessageContract.Tests
{
    internal static class MessageExpectationProvider
    {
        //Expectations on each property of each message type
        private readonly static object workflowCreatedContent = new
        {
            StartArg = Match.Type("Aim.Gain.StaticData.DataManagement.BusinessProcess.FreshAggregateRequestBusinessProcessStartArgument"),
            Description = Match.Type("Party from feed Excel-Party [IdBBCompany:abc]"),
            Priority = Match.Type(ProcessPriority.Normal),
            //Alarms = Match.Type(new Alarms()),
            Ancestry = Match.Type(Array.Empty<Ancestor>()),
            StartsBulkOperation = Match.Type(false),
            //BreaksBulkSuccession = Match.Type(false),
            //Args = Match.Type(new Dictionary<string, object>()),
            ContextData = Match.Type(new Dictionary<string, object>()),
            WorkflowType = Match.Type(WorkflowType.Business),
            WorkflowId = Match.Number(11),
            CorrelationId = Match.Type("sba"),
            //BulkCorrelationId = Match.Type("sba"),
            //WorkflowCorrelation = Match.Type("sba"),
        };

        private readonly static object workflowFinishedContent = new
        {
            State = Match.Type(WorkflowFinishedState.Finished),
            WorkflowId = Match.Number(123),
            WorkflowType = Match.Type(WorkflowType.Business),
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