using System;
using Overflow;
using WorkflowExperiment.Operations.IO;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Operations
{
    [ContinueOnFailure]
    public class SendEmailToHimOperation : Operation
    {
        private readonly IMailService _mailService;

        [Input]
        public SendEmailToHimInput Input { get; set; }

        public SendEmailToHimOperation(IMailService mailService)
        {
            _mailService = mailService;
        }

        protected override void OnExecute()
        {
            _mailService.Send(Input.Email, "Hello Man");
            Console.WriteLine($"Hello Man {Input.Email}!");
        }
    }
}