using System;
using System.Linq;
using WorkflowExperiment.Flows;
using WorkflowExperiment.Models;

namespace WorkflowExperimentConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			var flow = new RegisterUserFlow
			{
				User = new User
				{
					Email = "user@email.com",
					Password = "password"
				}
			};

			var errors = flow.Validate()
				.Errors.ToList();

			flow.Run();

			Console.ReadKey();
		}
	}
}
