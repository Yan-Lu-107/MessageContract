using SimCorp.Gain.Messages.Data;
using SimCorp.Gain.Messages.System.Batch;
using SimCorp.Gain.Messages.System.Operations;
using SimCorp.Gain.Messages.System.Task;
using SimCorp.Gain.Messages.System.Workflow;

namespace MessageContract.Tests;

public static class EventTypeMapper
{
    public static Type GetTypeForEventName(string messageName) => messageName switch
    {
        // Request related events
        "com.simcorp.gain.system.start_operation.v2" => typeof(StartOperation),
        "com.simcorp.gain.system.start_batch.v2" => typeof(StartBatch),
        "com.simcorp.gain.system.start_operation_failed.v1" => typeof(StartOperationFailed),
        "com.simcorp.gain.system.start_operation_succeeded.v1" => typeof(StartOperationSucceeded),

        // Task events
        "com.simcorp.gain.system.task_created.v2" => typeof(TaskCreated),
        "com.simcorp.gain.system.task_resolution_pending.v1" => typeof(TaskResolutionPending),

        // Workflow events
        "com.simcorp.gain.system.workflow_changed_business_object.v3" => typeof(WorkflowChangedBusinessObject),
        "com.simcorp.gain.system.workflow_created.v3" => typeof(WorkflowCreated),
        "com.simcorp.gain.system.workflow_finished.v2" => typeof(WorkflowFinished),
        "com.simcorp.gain.system.workflow_ready.v2" => typeof(WorkflowReady),
        "com.simcorp.gain.system.workflow_waiting.v2" => typeof(WorkflowWaiting),

        "com.simcorp.gain.data.business_object_changed.v5" => typeof(BusinessObjectChanged),

        // Default case
        _ => throw new ArgumentException($"{messageName} is not a recognised type name. Please consider adding this to {nameof(EventTypeMapper)}.")
    };
}
