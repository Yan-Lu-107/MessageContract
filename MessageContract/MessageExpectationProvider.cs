using PactNet.Matchers;
using SimCorp.Gain.Messages.Data;
using SimCorp.Gain.Messages.Shared.BusinessObject;
using SimCorp.Gain.Messages.Shared.Workflow;
using SimCorp.Gain.Messages.System.Operations;
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
            Ancestry = Match.Type(Array.Empty<Ancestor>()),
            StartsBulkOperation = Match.Type(false),
            ContextData = Match.Type(new Dictionary<string, object>()),
            WorkflowType = Match.Type(WorkflowType.Business),
            WorkflowId = Match.Number(11),
            CorrelationId = Match.Type("sba"),
        };

        private readonly static object workflowFinishedContent = new
        {
            State = Match.Type(WorkflowFinishedState.Finished),
            WorkflowId = Match.Number(123),
            WorkflowType = Match.Type(WorkflowType.Business),
            CorrelationId = Match.Type("abc")
        };

        private readonly static object workflowChangedBusinessObjectContent = new
        {
            BusinessObjectReference = Match.Type(
                new BusinessObjectReference()
                {
                    Model = "SimCorpDimension",
                    DataType = "Party",
                    Domain = "Golden",
                    Id = 123,
                    Version = 1,
                }),
            Title = Match.Include("GP_5232"),
            Discriminator = "Master",
            WorkflowId = Match.Number(123),
            WorkflowType = Match.Type(WorkflowType.Business),
            CorrelationId = Match.Type("aa")
        };

        private readonly static object startOperationFailedContent = new
        {
        };
        private readonly static object startOperationSucceededContent = new
        {
        };
        private readonly static object workflowReadyContent = new
        {
        };
        private readonly static object workflowWaitingContent = new
        {
            WaitingFor = Match.Type(WorkflowWaitingFor.Workflow),
            WorkflowId = Match.Number(123),
            CorrelationId = Match.Type("aa")
        };
        private readonly static object businessObjectChangedContent = new
        {
        };
        private readonly static Dictionary<Type, object> expectedEventDict = new Dictionary<Type, object>
        {
            { typeof(StartOperationFailed) , startOperationFailedContent },
            { typeof(StartOperationSucceeded) , startOperationSucceededContent },

            { typeof(WorkflowCreated) , workflowCreatedContent  },
            { typeof(WorkflowFinished), workflowFinishedContent },
            { typeof(WorkflowReady), workflowReadyContent },
            { typeof(WorkflowWaiting), workflowWaitingContent },
            { typeof(WorkflowChangedBusinessObject), workflowChangedBusinessObjectContent },

            { typeof(BusinessObjectChanged), businessObjectChangedContent }

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