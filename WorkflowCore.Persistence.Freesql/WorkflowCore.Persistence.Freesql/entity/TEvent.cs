using FreeSql.DataAnnotations;

namespace WorkflowCore.Persistence.Freesql.entity;

[Table(Name = "wfc_event")]
[Index("IX_Event_EventId", "EventId", true)]
[Index("IX_Event_EventTime", "EventTime", false)]
[Index("IX_Event_IsProcessed", "IsProcessed", false)]
[Index("IX_Event_EventName_EventKey", "EventName,EventKey", false)]
public class TEvent
{
	[Column(IsIdentity = true, IsPrimary = true)]
	public long PersistenceId { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string EventData { get; set; }

	[Column(StringLength = 64,IsNullable = false)]
	public string EventId { get; set; }

	[Column(StringLength = 200)]
	public string EventName { get; set; }

	[Column(StringLength = 200)]
	public string EventKey { get; set; }

	[Column(IsNullable = false)]
	public DateTime EventTime { get; set; }

	[Column(IsNullable = false)]
	public bool IsProcessed { get; set; }
}