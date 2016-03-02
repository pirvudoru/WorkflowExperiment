using System;
using System.Threading.Tasks;
using Banzai;
using WorkflowExperiment.Nodes.IO;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Nodes
{
    public class SendEmailToHerNode : Node<SendEmailToHerContext>
    {
        private readonly IMailService _mailService;

        public SendEmailToHerNode(IMailService mailService)
        {
            _mailService = mailService;
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<SendEmailToHerContext> context)
        {
            var email = context.Subject.Email;

            _mailService.Send(email, "Hello Miss");
            Console.WriteLine($"Hello Man {email}!");

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }
}