using Newtonsoft.Json;
using WorkflowCore.Models;
using WorkflowCore.Persistence.Freesql.entity;

namespace WorkflowCore.Persistence.Freesql;

public static class EntityConverter
{
	private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
		{ TypeNameHandling = TypeNameHandling.All };

	public static string ToJson(this object obj)
	{
		return JsonConvert.SerializeObject(obj, SerializerSettings);
	}
	
	public static T FromJson<T>(this string obj)
	{
		return JsonConvert.DeserializeObject<T>(obj, SerializerSettings);
	}

	public static TWorkflow ToPersistable(this WorkflowInstance instance, TWorkflow persistable = null)
	{
		persistable ??= new TWorkflow();
		persistable.Data = JsonConvert.SerializeObject(instance.Data, SerializerSettings);
		persistable.Description = instance.Description;
		persistable.Reference = instance.Reference;
		persistable.InstanceId = instance.Id;
		persistable.NextExecution = instance.NextExecution;
		persistable.Version = instance.Version;
		persistable.WorkflowDefinitionId = instance.WorkflowDefinitionId;
		persistable.Status = instance.Status;
		persistable.CreateTime = instance.CreateTime;
		persistable.CompleteTime = instance.CompleteTime;

		foreach (var ep in instance.ExecutionPointers)
		{
			var epId = ep.Id ?? Guid.NewGuid().ToString();
			if (!persistable.ExecutionPointers.TryGetValue(epId, out var persistedEP))
			{
				persistedEP = new MExecutionPointer
				{
					Id = epId
				};
				persistable.ExecutionPointers.Add(epId, persistedEP);
			}

			persistedEP.StepId = ep.StepId;
			persistedEP.Active = ep.Active;
			persistedEP.SleepUntil = ep.SleepUntil;
			persistedEP.PersistenceData = JsonConvert.SerializeObject(ep.PersistenceData, SerializerSettings);
			persistedEP.StartTime = ep.StartTime;
			persistedEP.EndTime = ep.EndTime;
			persistedEP.StepName = ep.StepName;
			persistedEP.RetryCount = ep.RetryCount;
			persistedEP.PredecessorId = ep.PredecessorId;
			persistedEP.ContextItem = JsonConvert.SerializeObject(ep.ContextItem, SerializerSettings);
			persistedEP.Children = string.Empty;

			foreach (var child in ep.Children)
				persistedEP.Children += child + ";";

			persistedEP.EventName = ep.EventName;
			persistedEP.EventKey = ep.EventKey;
			persistedEP.EventPublished = ep.EventPublished;
			persistedEP.EventData = JsonConvert.SerializeObject(ep.EventData, SerializerSettings);
			persistedEP.Outcome = JsonConvert.SerializeObject(ep.Outcome, SerializerSettings);
			persistedEP.Status = ep.Status;

			persistedEP.Scope = string.Empty;
			foreach (var item in ep.Scope)
				persistedEP.Scope += item + ";";

			foreach (var attr in ep.ExtensionAttributes)
			{
				var persistedAttr = persistedEP.ExtensionAttributes.FirstOrDefault(x => x.AttributeKey == attr.Key);
				if (persistedAttr == null)
				{
					persistedAttr = new MExtensionAttribute();
					persistedEP.ExtensionAttributes.Add(persistedAttr);
				}

				persistedAttr.AttributeKey = attr.Key;
				persistedAttr.AttributeValue = JsonConvert.SerializeObject(attr.Value, SerializerSettings);
			}
		}
		return persistable;
	}

	public static WorkflowInstance ToWorkflowInstance(this TWorkflow instance)
	{
		var result = new WorkflowInstance
		{
			Data = JsonConvert.DeserializeObject(instance.Data, SerializerSettings),
			Description = instance.Description,
			Reference = instance.Reference,
			Id = instance.InstanceId,
			NextExecution = instance.NextExecution,
			Version = instance.Version,
			WorkflowDefinitionId = instance.WorkflowDefinitionId,
			Status = instance.Status,
			CreateTime = DateTime.SpecifyKind(instance.CreateTime, DateTimeKind.Utc)
		};
		if (instance.CompleteTime.HasValue)
			result.CompleteTime = DateTime.SpecifyKind(instance.CompleteTime.Value, DateTimeKind.Utc);

		result.ExecutionPointers = new ExecutionPointerCollection(instance.ExecutionPointers.Count + 8);

		foreach (var ep in instance.ExecutionPointers.Values)
		{
			var pointer = new ExecutionPointer
			{
				Id = ep.Id,
				StepId = ep.StepId,
				Active = ep.Active
			};

			if (ep.SleepUntil.HasValue)
				pointer.SleepUntil = DateTime.SpecifyKind(ep.SleepUntil.Value, DateTimeKind.Utc);

			pointer.PersistenceData =
				JsonConvert.DeserializeObject(ep.PersistenceData ?? string.Empty, SerializerSettings);

			if (ep.StartTime.HasValue)
				pointer.StartTime = DateTime.SpecifyKind(ep.StartTime.Value, DateTimeKind.Utc);

			if (ep.EndTime.HasValue)
				pointer.EndTime = DateTime.SpecifyKind(ep.EndTime.Value, DateTimeKind.Utc);

			pointer.StepName = ep.StepName;

			pointer.RetryCount = ep.RetryCount;
			pointer.PredecessorId = ep.PredecessorId;
			pointer.ContextItem = JsonConvert.DeserializeObject(ep.ContextItem ?? string.Empty, SerializerSettings);

			if (!string.IsNullOrEmpty(ep.Children))
				pointer.Children = ep.Children.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

			pointer.EventName = ep.EventName;
			pointer.EventKey = ep.EventKey;
			pointer.EventPublished = ep.EventPublished;
			pointer.EventData = JsonConvert.DeserializeObject(ep.EventData ?? string.Empty, SerializerSettings);
			pointer.Outcome = JsonConvert.DeserializeObject(ep.Outcome ?? string.Empty, SerializerSettings);
			pointer.Status = ep.Status;

			if (!string.IsNullOrEmpty(ep.Scope))
			{
				var arrs = ep.Scope.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				pointer.Scope = new List<string>(arrs);
			}

			foreach (var attr in ep.ExtensionAttributes)
			{
				pointer.ExtensionAttributes[attr.AttributeKey] =
					JsonConvert.DeserializeObject(attr.AttributeValue, SerializerSettings);
			}

			result.ExecutionPointers.Add(pointer);
		}

		return result;
	}

	public static TSubscription ToPersistable(this EventSubscription instance)
	{
		var result = new TSubscription
		{
			SubscriptionId = instance.Id,
			EventKey = instance.EventKey,
			EventName = instance.EventName,
			StepId = instance.StepId,
			ExecutionPointerId = instance.ExecutionPointerId,
			WorkflowId = instance.WorkflowId,
			SubscribeAsOf = DateTime.SpecifyKind(instance.SubscribeAsOf, DateTimeKind.Utc),
			SubscriptionData = JsonConvert.SerializeObject(instance.SubscriptionData, SerializerSettings),
			ExternalToken = instance.ExternalToken,
			ExternalTokenExpiry = instance.ExternalTokenExpiry,
			ExternalWorkerId = instance.ExternalWorkerId
		};

		return result;
	}

	public static EventSubscription ToEventSubscription(this TSubscription instance)
	{
		var result = new EventSubscription
		{
			Id = instance.SubscriptionId,
			EventKey = instance.EventKey,
			EventName = instance.EventName,
			StepId = instance.StepId,
			ExecutionPointerId = instance.ExecutionPointerId,
			WorkflowId = instance.WorkflowId,
			SubscribeAsOf = DateTime.SpecifyKind(instance.SubscribeAsOf, DateTimeKind.Utc),
			SubscriptionData = JsonConvert.DeserializeObject(instance.SubscriptionData, SerializerSettings),
			ExternalToken = instance.ExternalToken,
			ExternalTokenExpiry = instance.ExternalTokenExpiry,
			ExternalWorkerId = instance.ExternalWorkerId
		};

		return result;
	}


	public static TEvent ToPersistable(this Event instance)
	{
		var result = new TEvent
		{
			EventId = instance.Id,
			EventKey = instance.EventKey,
			EventName = instance.EventName,
			EventTime = DateTime.SpecifyKind(instance.EventTime, DateTimeKind.Utc),
			IsProcessed = instance.IsProcessed,
			EventData = JsonConvert.SerializeObject(instance.EventData, SerializerSettings)
		};

		return result;
	}

	public static TScheduledCommand ToPersistable(this ScheduledCommand instance)
	{
		var result = new TScheduledCommand
		{
			CommandName = instance.CommandName,
			Data = instance.Data,
			ExecuteTime = instance.ExecuteTime
		};

		return result;
	}

	public static Event ToEvent(this TEvent instance)
	{
		var result = new Event
		{
			Id = instance.EventId,
			EventKey = instance.EventKey,
			EventName = instance.EventName,
			EventTime = DateTime.SpecifyKind(instance.EventTime, DateTimeKind.Utc),
			IsProcessed = instance.IsProcessed,
			EventData = JsonConvert.DeserializeObject(instance.EventData, SerializerSettings)
		};

		return result;
	}

	public static ScheduledCommand ToScheduledCommand(this TScheduledCommand instance)
	{
		var result = new ScheduledCommand
		{
			CommandName = instance.CommandName,
			Data = instance.Data,
			ExecuteTime = instance.ExecuteTime
		};

		return result;
	}

	public static TExecutionError ToPersistable(this ExecutionError instance)
	{
		var result = new TExecutionError
		{
			ErrorTime = instance.ErrorTime,
			Message = instance.Message,
			ExecutionPointerId = instance.ExecutionPointerId,
			WorkflowId = instance.WorkflowId
		};

		return result;
	}
}