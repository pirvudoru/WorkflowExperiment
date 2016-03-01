using System;
using MicroFlow;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Activities
{
	public class SubscribeActivity : SyncActivity
	{
		private readonly ISubscriptionService _subscriptionService;

		[Required]
		public int Id { get; set; }

		public SubscribeActivity(ISubscriptionService subscriptionService)
		{
			_subscriptionService = subscriptionService;
		}

		protected override void ExecuteActivity()
		{
			_subscriptionService.Subscribe(Id);

			Console.WriteLine($"Subscribing user {Id}");
		}
	}
}