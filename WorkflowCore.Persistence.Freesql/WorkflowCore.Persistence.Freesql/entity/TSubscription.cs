using FreeSql.DataAnnotations;

namespace WorkflowCore.Persistence.Freesql.entity;
[Table(Name = "wfc_subscription")]
[Index("IX_Subscription_SubscriptionId", "SubscriptionId", true)]
[Index("IX_Subscription_EventKey", "EventKey", false)]
[Index("IX_Subscription_EventName", "EventName", false)]
public class TSubscription
{
	[Column(IsIdentity = true, IsPrimary = true)]
	public long PersistenceId { get; set; }
	
	[Column(StringLength = 200)]
	public string EventKey { get; set; }
	
	[Column(StringLength = 200)]
	public string EventName { get; set; }

	[Column(IsNullable = false)]
	public int StepId { get; set; }
	
	[Column(StringLength = 64,IsNullable = false)]
	public string SubscriptionId { get; set; }

	[Column(StringLength = 200)]
	public string WorkflowId { get; set; }

	[Column(IsNullable = false)]
	public DateTime SubscribeAsOf { get; set; }

	[Column(StringLength = 200)]
	public string ExecutionPointerId { get; set; }

	[Column(StringLength = 200)]
	public string ExternalToken { get; set; }

	public DateTime? ExternalTokenExpiry { get; set; }

	[Column(StringLength = 200)]
	public string ExternalWorkerId { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string SubscriptionData { get; set; }
}