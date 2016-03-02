using System;
using Overflow;
using WorkflowExperiment.Operations.IO;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Operations
{
    [ContinueOnFailure]
    public class SubscribeOperation : Operation
    {
        private readonly ISubscriptionService _subscriptionService;

        [Input]
        public SubscribeInput Input { get; set; }

        public SubscribeOperation(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        protected override void OnExecute()
        {
            _subscriptionService.Subscribe(Input.UserId);

            Console.WriteLine($"Subscribing user {Input.UserId}");
        }
    }
}