using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Persistence.Freesql.entity;

namespace WorkflowCore.Persistence.Freesql;

public class WorkflowPurger : IWorkflowPurger
{
	private readonly ILogger _logger;
	private readonly IFreeSql _fsql;

	public WorkflowPurger(IFreeSql fsql, ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<FreeSqlPersistenceProvider>();
		_fsql = fsql;
	}

	public async Task PurgeWorkflows(WorkflowStatus status, DateTime olderThan, CancellationToken cancellationToken)
	{
		var olderThanUtc = olderThan.ToUniversalTime();
		var removeCount = await _fsql.Delete<TWorkflow>()
			.Where(x => x.Status == status && x.CompleteTime < olderThanUtc)
			.ExecuteAffrowsAsync(cancellationToken);
		_logger.Log(LogLevel.Information, "purge {Count} workflows", removeCount);
	}
}