using System;
using MicroFlow;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Activities
{
	public class SendEmailToHimActivity : SyncActivity
	{
		private readonly IMailService _mailService;

		[Required]
		public string Email { get; set; }

		public SendEmailToHimActivity(IMailService mailService)
		{
			_mailService = mailService;
		}

		protected override void ExecuteActivity()
		{
			_mailService.Send(Email, "Hello Man");
			Console.WriteLine($"Hello Man {Email}!");
		}
	}
}