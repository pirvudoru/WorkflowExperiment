using System;
using MicroFlow;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Activities
{
	public class SendEmailToHerActivity : SyncActivity
	{
		private readonly IMailService _mailService;

		[Required]
		public string Email { get; set; }

		public SendEmailToHerActivity(IMailService mailService)
		{
			_mailService = mailService;
		}

		protected override void ExecuteActivity()
		{
			_mailService.Send(Email, "Hello Miss");
			Console.WriteLine($"Hello Man {Email}!");
		}
	}
}