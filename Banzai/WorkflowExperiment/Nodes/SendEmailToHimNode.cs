using System;
using System.Threading.Tasks;
using Banzai;
using WorkflowExperiment.Nodes.IO;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Nodes
{
    public class SendEmailToHimNode : Node<SendEmailToHimContext>
    {
        private readonly IMailService _mailService;

        public SendEmailToHimNode(IMailService mailService)
        {
            _mailService = mailService;
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<SendEmailToHimContext> context)
        {
            var email = context.Subject.Email;

            _mailService.Send(email, "Hello Man");
            Console.WriteLine($"Hello Man {email}!");

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }
}