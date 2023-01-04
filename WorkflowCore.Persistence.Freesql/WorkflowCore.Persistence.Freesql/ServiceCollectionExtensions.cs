using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Persistence.Freesql;

public static class ServiceCollectionExtensions
{
	public static WorkflowOptions UseFreeSql(this WorkflowOptions options, bool canSyncStructure)
	{
		options.UsePersistence(sp => CreatePersistenceProvider(sp, canSyncStructure));
		options.Services.AddTransient<IWorkflowPurger, WorkflowPurger>();
		return options;
	}

	private static IPersistenceProvider CreatePersistenceProvider(IServiceProvider sp, bool canSyncStructure)
	{
		var fsql = sp.GetService<IFreeSql>();
		var loggerFactory = sp.GetService<ILoggerFactory>();
		return new FreeSqlPersistenceProvider(fsql, canSyncStructure, loggerFactory);
	}
}