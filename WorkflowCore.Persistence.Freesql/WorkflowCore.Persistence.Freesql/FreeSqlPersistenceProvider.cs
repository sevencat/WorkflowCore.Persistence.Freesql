using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Persistence.Freesql;

public class FreeSqlPersistenceProvider : IPersistenceProvider
{
	private ILogger _logger;
	private IFreeSql _fsql;
	private bool _canSyncDbStruct;

	public FreeSqlPersistenceProvider(IFreeSql fsql, bool canSyncDbStruct, ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<FreeSqlPersistenceProvider>();
		_fsql = fsql;
		_canSyncDbStruct = canSyncDbStruct;
	}

	public Task<string> CreateNewWorkflow(WorkflowInstance workflow,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task PersistWorkflow(WorkflowInstance workflow,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task PersistWorkflow(WorkflowInstance workflow, List<EventSubscription> subscriptions,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(WorkflowStatus? status, string type,
		DateTime? createdFrom, DateTime? createdTo, int skip,
		int take)
	{
		throw new NotImplementedException();
	}

	public Task<WorkflowInstance> GetWorkflowInstance(string Id,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(IEnumerable<string> ids,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<string> CreateEventSubscription(EventSubscription subscription,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<EventSubscription>> GetSubscriptions(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task TerminateSubscription(string eventSubscriptionId,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<EventSubscription> GetSubscription(string eventSubscriptionId,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<EventSubscription> GetFirstOpenSubscription(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<bool> SetSubscriptionToken(string eventSubscriptionId, string token, string workerId, DateTime expiry,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task ClearSubscriptionToken(string eventSubscriptionId, string token,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<string> CreateEvent(Event newEvent, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<Event> GetEvent(string id, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task MarkEventProcessed(string id, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task MarkEventUnprocessed(string id, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task ScheduleCommand(ScheduledCommand command)
	{
		throw new NotImplementedException();
	}

	public Task ProcessCommands(DateTimeOffset asOf, Func<ScheduledCommand, Task> action,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public bool SupportsScheduledCommands { get; }

	public Task PersistErrors(IEnumerable<ExecutionError> errors,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public void EnsureStoreExists()
	{
		throw new NotImplementedException();
	}
}