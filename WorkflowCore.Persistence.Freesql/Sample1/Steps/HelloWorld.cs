using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Sample1.Steps
{
    public class HelloWorld : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Hello world");
            Task.Delay(10000).Wait();
            Console.WriteLine("Hello world end");
            return ExecutionResult.Next();
        }
    }
}
