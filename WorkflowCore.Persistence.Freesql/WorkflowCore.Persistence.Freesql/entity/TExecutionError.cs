using FreeSql.DataAnnotations;

namespace WorkflowCore.Persistence.Freesql.entity;

[Table(Name = "wfc_executionerror")]
public class TExecutionError
{
	[Column(IsIdentity = true, IsPrimary = true)]
	public long PersistenceId { get; set; }

	[Column(IsNullable = false)]
	public DateTime ErrorTime { get; set; }

	[Column(StringLength = 100)]
	public string ExecutionPointerId { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string Message { get; set; }

	[Column(StringLength = 100)]
	public string WorkflowId { get; set; }
}