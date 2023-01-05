using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Sample1.Steps
{
    public class GoodbyeWorld : StepBody
    {

        private ILogger _logger;

        public GoodbyeWorld(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GoodbyeWorld>();
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Goodbye world");
            Task.Delay(10000).Wait();
            _logger.LogInformation("Hi there!");
            return ExecutionResult.Next();
        }
    }
}
