using Microsoft.Extensions.DependencyInjection;
using Sample1.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Persistence.Freesql;
using WorkflowCore.Sample01;

namespace Sample1;

class Program
{
	public static void Main(string[] args)
	{
		IServiceProvider serviceProvider = ConfigureServices();

		//start the workflow host
		var host = serviceProvider.GetService<IWorkflowHost>();
		host.RegisterWorkflow<HelloWorldWorkflow>();
		host.Start();

		//把这行注释测试程序重启恢复功能
		host.StartWorkflow("HelloWorld");

		Console.ReadLine();
		host.Stop();
	}

	private static IServiceProvider ConfigureServices()
	{
		//setup dependency injection
		IServiceCollection services = new ServiceCollection();
		services.AddLogging();
		
		var connstr =
			@"Server=127.0.0.1;Database=wfc;User=root;Password=123456;";
		var fsql = new FreeSql.FreeSqlBuilder()
			.UseConnectionString(FreeSql.DataType.MySql, connstr)
			.UseAutoSyncStructure(false)
			.Build();
		services.AddSingleton(fsql);
		
		services.AddWorkflow(x => x.UseFreeSql(true));
		services.AddTransient<GoodbyeWorld>();

		var serviceProvider = services.BuildServiceProvider();

		return serviceProvider;
	}
}