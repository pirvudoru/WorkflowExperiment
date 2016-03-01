using System;
using MicroFlow;

namespace WorkflowExperiment.Framework
{
	public class DefaultCancelationHandler : SyncActivity
	{
		protected override void ExecuteActivity()
		{
			Console.WriteLine("Cancelled");
		}
	}
}