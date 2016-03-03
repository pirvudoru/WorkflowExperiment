using System;
using Overflow;
using WorkflowExperiment.Models;
using WorkflowExperiment.Operations;
using WorkflowExperiment.Operations.IO;

namespace WorkflowExperimentConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var simpleOperationResolver = new SimpleOperationResolver();
            var textWriterWorkflowLogger = new TextWriterWorkflowLogger(Console.Out);
            var configuration = Workflow.Configure<RegisterUserStateMachine>()
                .WithLogger(textWriterWorkflowLogger)
                .WithResolver(simpleOperationResolver);

            var flow = new RegisterUserStateMachine
            {
                Input = new RegisterUserInput
                {
                    User = new User
                    {
                        Email = "user@email.com",
                        Password = "password"
                    }
                }
            };
            flow.Initialize(configuration);
            flow.Execute();

            Console.WriteLine(flow.Output.UserId);

            Console.ReadKey();
        }
    }
}