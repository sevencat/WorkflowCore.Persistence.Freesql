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

	public async Task<string> CreateNewWorkflow(WorkflowInstance workflow, CancellationToken cancellationToken)
	{
		workflow.Id = Guid.NewGuid().ToString();
		var persistable = workflow.ToPersistable();
		persistable.BeforeInsert();
		await _fsql.Insert(persistable).ExecuteIdentityAsync(cancellationToken);
		return workflow.Id;
	}

	public async Task PersistWorkflow(WorkflowInstance workflow, CancellationToken cancellationToken)
	{
		var existingEntity = await _fsql.Select<TWorkflow>()
			.Where(x => x.InstanceId == workflow.Id)
			.FirstAsync(cancellationToken);
		existingEntity?.AfterSelect();
		var persistable = workflow.ToPersistable(existingEntity);
		persistable.BeforeInsert();
		await _fsql.InsertOrUpdate<TWorkflow>().SetSource(persistable).ExecuteAffrowsAsync(cancellationToken);
	}

	public async Task PersistWorkflow(WorkflowInstance workflow, List<EventSubscription> subscriptions,
		CancellationToken cancellationToken)
	{
		var existingEntity = await _fsql.Select<TWorkflow>()
			.Where(x => x.InstanceId == workflow.Id)
			.FirstAsync(cancellationToken);
		existingEntity?.AfterSelect();
		var persistable = workflow.ToPersistable(existingEntity);
		var subscriptionPersistables = subscriptions.Select((x) =>
		{
			x.Id = Guid.NewGuid().ToString();
			var subscriptionPersistable = x.ToPersistable();
			return subscriptionPersistable;
		}).ToList();
		_fsql.Transaction(() =>
		{
			persistable.BeforeInsert();
			_fsql.InsertOrUpdate<TWorkflow>().SetSource(persistable).ExecuteAffrows();
			_fsql.Insert(subscriptionPersistables).ExecuteAffrows();
		});
	}

	public async Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt,
		CancellationToken cancellationToken)
	{
		var now = asAt.ToUniversalTime().Ticks;
		var dbitems = await _fsql.Select<TWorkflow>()
			.Where(x => x.NextExecution.HasValue && (x.NextExecution <= now) && (x.Status == WorkflowStatus.Runnable))
			.ToListAsync(x => x.InstanceId, cancellationToken);
		return dbitems;
	}

	public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(WorkflowStatus? status, string type,
		DateTime? createdFrom, DateTime? createdTo, int skip, int take)
	{
		var query = _fsql.Select<TWorkflow>();
		if (status.HasValue)
			query = query.Where(x => x.Status == status.Value);

		if (!String.IsNullOrEmpty(type))
			query = query.Where(x => x.WorkflowDefinitionId == type);

		if (createdFrom.HasValue)
			query = query.Where(x => x.CreateTime >= createdFrom.Value);

		if (createdTo.HasValue)
			query = query.Where(x => x.CreateTime <= createdTo.Value);

		var dbitems = await query.Skip(skip).Take(take).ToListAsync();
		return dbitems.Select((x) =>
		{
			x.AfterSelect();
			return x.ToWorkflowInstance();
		});
	}

	public async Task<WorkflowInstance> GetWorkflowInstance(string Id, CancellationToken cancellationToken)
	{
		var existingEntity = await _fsql.Select<TWorkflow>()
			.Where(x => x.InstanceId == Id)
			.FirstAsync(cancellationToken);
		existingEntity?.AfterSelect();
		return existingEntity?.ToWorkflowInstance();
	}

	public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(IEnumerable<string> ids,
		CancellationToken cancellationToken)
	{
		if (ids == null)
		{
			return new List<WorkflowInstance>();
		}

		var dbitems = await _fsql.Select<TWorkflow>()
			.Where(x => ids.Contains(x.InstanceId))
			.ToListAsync(cancellationToken);
		return dbitems.Select((x) =>
		{
			x.AfterSelect();
			return x.ToWorkflowInstance();
		});
	}

	public async Task<string> CreateEventSubscription(EventSubscription subscription,
		CancellationToken cancellationToken)
	{
		subscription.Id = Guid.NewGuid().ToString();
		var persistable = subscription.ToPersistable();
		await _fsql.Insert(persistable).ExecuteIdentityAsync(cancellationToken);
		return subscription.Id;
	}

	public async Task<IEnumerable<EventSubscription>> GetSubscriptions(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken)
	{
		asOf = asOf.ToUniversalTime();
		var dbitems = await _fsql.Select<TSubscription>()
			.Where(x => x.EventName == eventName && x.EventKey == eventKey && x.SubscribeAsOf <= asOf)
			.ToListAsync(cancellationToken);
		return dbitems.Select(item => item.ToEventSubscription()).ToList();
	}

	public async Task TerminateSubscription(string eventSubscriptionId,
		CancellationToken cancellationToken)
	{
		await _fsql.Delete<TSubscription>().Where(x => x.SubscriptionId == eventSubscriptionId)
			.ExecuteAffrowsAsync(cancellationToken);
	}

	public async Task<EventSubscription> GetSubscription(string eventSubscriptionId,
		CancellationToken cancellationToken)
	{
		var dbitem = await _fsql.Select<TSubscription>().Where(x => x.SubscriptionId == eventSubscriptionId)
			.FirstAsync(cancellationToken);
		return dbitem?.ToEventSubscription();
	}

	public async Task<EventSubscription> GetFirstOpenSubscription(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken)
	{
		var dbitem = await _fsql.Select<TSubscription>()
			.Where(x => x.EventName == eventName && x.EventKey == eventKey && x.SubscribeAsOf <= asOf &&
			            x.ExternalToken == null)
			.FirstAsync(cancellationToken);
		return dbitem?.ToEventSubscription();
	}

	public async Task<bool> SetSubscriptionToken(string eventSubscriptionId, string token, string workerId,
		DateTime expiry,
		CancellationToken cancellationToken)
	{
		var rows = await _fsql.Update<TSubscription>()
			.Set(x => x.ExternalToken, token)
			.Set(x => x.ExternalWorkerId, workerId)
			.Set(x => x.ExternalTokenExpiry, expiry)
			.Where(x => x.SubscriptionId == eventSubscriptionId)
			.ExecuteAffrowsAsync(cancellationToken);

		return rows > 0;
	}

	public async Task ClearSubscriptionToken(string eventSubscriptionId, string token,
		CancellationToken cancellationToken)
	{
		await _fsql.Update<TSubscription>()
			.Set(x => x.ExternalToken, null)
			.Set(x => x.ExternalWorkerId, null)
			.Set(x => x.ExternalTokenExpiry, null)
			.Where(x => x.SubscriptionId == eventSubscriptionId)
			.ExecuteAffrowsAsync(cancellationToken);
	}

	public async Task<string> CreateEvent(Event newEvent, CancellationToken cancellationToken)
	{
		newEvent.Id = Guid.NewGuid().ToString();
		var persistable = newEvent.ToPersistable();
		await _fsql.Insert(persistable).ExecuteIdentityAsync(cancellationToken);
		return newEvent.Id;
	}

	public async Task<Event> GetEvent(string id, CancellationToken cancellationToken)
	{
		var dbitem = await _fsql.Select<TEvent>().Where(x => x.EventId == id).FirstAsync(cancellationToken);
		return dbitem?.ToEvent();
	}

	public async Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt,
		CancellationToken cancellationToken)
	{
		var now = asAt.ToUniversalTime();
		asAt = asAt.ToUniversalTime();
		var dbitems = await _fsql.Select<TEvent>()
			.Where(x => !x.IsProcessed)
			.Where(x => x.EventTime <= now)
			.ToListAsync(x => x.EventId, cancellationToken);
		return dbitems.Select(s => s.ToString()).ToList();
	}

	public async Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken)
	{
		var dbitems = await _fsql.Select<TEvent>()
			.Where(x => x.EventName == eventName && x.EventKey == eventKey)
			.Where(x => x.EventTime >= asOf)
			.ToListAsync(x => x.EventId, cancellationToken);

		var result = new List<string>();
		foreach (var s in dbitems)
			result.Add(s);
		return result;
	}

	public async Task MarkEventProcessed(string id, CancellationToken cancellationToken)
	{
		await _fsql.Update<TEvent>()
			.Set(x => x.IsProcessed, true)
			.Where(x => x.EventId == id)
			.ExecuteAffrowsAsync(cancellationToken);
	}

	public async Task MarkEventUnprocessed(string id, CancellationToken cancellationToken)
	{
		await _fsql.Update<TEvent>()
			.Set(x => x.IsProcessed, false)
			.Where(x => x.EventId == id)
			.ExecuteAffrowsAsync(cancellationToken);
	}

	public async Task ScheduleCommand(ScheduledCommand command)
	{
		var persistable = command.ToPersistable();
		await _fsql.Insert(persistable).ExecuteIdentityAsync();
	}

	public Task ProcessCommands(DateTimeOffset asOf, Func<ScheduledCommand, Task> action,
		CancellationToken cancellationToken)
	{
		var cursor = _fsql.Select<TScheduledCommand>()
			.Where(x => x.ExecuteTime < asOf.UtcDateTime.Ticks)
			.ToList();

		foreach (var command in cursor)
		{
			try
			{
				action(command.ToScheduledCommand());
				_fsql.Delete<TScheduledCommand>().Where(x => x.PersistenceId == command.PersistenceId)
					.ExecuteAffrows();
			}
			catch (Exception)
			{
				//TODO
			}
		}

		return Task.CompletedTask;
	}

	public bool SupportsScheduledCommands { get; } = true;

	public Task PersistErrors(IEnumerable<ExecutionError> errors, CancellationToken cancellationToken)
	{
		var executionErrors = errors as ExecutionError[] ?? errors.ToArray();
		if (executionErrors.Any())
		{
			_fsql.Transaction(() =>
			{
				foreach (var error in executionErrors)
				{
					_fsql.Insert(error.ToPersistable()).ExecuteIdentity();
				}
			});
		}
		return Task.CompletedTask;
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