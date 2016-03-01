using System;
using MicroFlow;

namespace WorkflowExperiment.Framework
{
	public class DefaultFaultHandler : SyncActivity, IFaultHandlerActivity
	{
		private readonly ILogger _logger;
		public Exception Exception { get; set; }

		public DefaultFaultHandler(ILogger logger)
		{
			_logger = logger;
		}

		protected override void ExecuteActivity()
		{
			_logger.Exception(Exception);
		}
	}
}