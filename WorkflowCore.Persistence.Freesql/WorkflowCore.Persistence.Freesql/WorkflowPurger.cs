using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Persistence.Freesql;

public class WorkflowPurger : IWorkflowPurger
{
	private ILogger _logger;
	private IFreeSql _fsql;

	public WorkflowPurger(IFreeSql fsql, ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<FreeSqlPersistenceProvider>();
		_fsql = fsql;
	}

	public Task PurgeWorkflows(WorkflowStatus status, DateTime olderThan, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}