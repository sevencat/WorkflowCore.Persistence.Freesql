using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Persistence.Freesql.entity;

namespace WorkflowCore.Persistence.Freesql;

public class FreeSqlPersistenceProvider : IPersistenceProvider
{
	private readonly ILogger _logger;
	private readonly IFreeSql _fsql;
	private readonly bool _canSyncDbStruct;

	public FreeSqlPersistenceProvider(IFreeSql fsql, bool canSyncDbStruct, ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<FreeSqlPersistenceProvider>();
		_fsql = fsql;
		_canSyncDbStruct = canSyncDbStruct;
	}

	public Task<string> CreateNewWorkflow(WorkflowInstance workflow, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task PersistWorkflow(WorkflowInstance workflow, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task PersistWorkflow(WorkflowInstance workflow, List<EventSubscription> subscriptions,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(WorkflowStatus? status, string type,
		DateTime? createdFrom, DateTime? createdTo, int skip,
		int take)
	{
		throw new NotImplementedException();
	}

	public Task<WorkflowInstance> GetWorkflowInstance(string Id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(IEnumerable<string> ids,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<string> CreateEventSubscription(EventSubscription subscription,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<EventSubscription>> GetSubscriptions(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task TerminateSubscription(string eventSubscriptionId,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<EventSubscription> GetSubscription(string eventSubscriptionId,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<EventSubscription> GetFirstOpenSubscription(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<bool> SetSubscriptionToken(string eventSubscriptionId, string token, string workerId, DateTime expiry,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task ClearSubscriptionToken(string eventSubscriptionId, string token,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<string> CreateEvent(Event newEvent, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<Event> GetEvent(string id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task MarkEventProcessed(string id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task MarkEventUnprocessed(string id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task ScheduleCommand(ScheduledCommand command)
	{
		throw new NotImplementedException();
	}

	public Task ProcessCommands(DateTimeOffset asOf, Func<ScheduledCommand, Task> action,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public bool SupportsScheduledCommands { get; } = true;

	public Task PersistErrors(IEnumerable<ExecutionError> errors, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public void EnsureStoreExists()
	{
		if (_canSyncDbStruct)
		{
			_logger.Log(LogLevel.Information, "auto create database");
			var cf = _fsql.CodeFirst;
			cf.SyncStructure<TEvent>();
			cf.SyncStructure<TExecutionError>();
			cf.SyncStructure<TScheduledCommand>();
			cf.SyncStructure<TSubscription>();
			cf.SyncStructure<TWorkflow>();
		}
	}
}