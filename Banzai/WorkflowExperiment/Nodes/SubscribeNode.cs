using System;
using System.Threading.Tasks;
using Banzai;
using WorkflowExperiment.Nodes.IO;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Nodes
{
    public class SubscribeNode : Node<SubscribeContext>
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscribeNode(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<SubscribeContext> context)
        {
            var userId = context.Subject.UserId;

            _subscriptionService.Subscribe(userId);
            Console.WriteLine($"Subscribing user {userId}");

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }
}