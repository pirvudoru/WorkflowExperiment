using System;
using Overflow;
using WorkflowExperiment.Operations.IO;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Operations
{
    [ContinueOnFailure]
    public class SendEmailToHerOperation : Operation
    {
        private readonly IMailService _mailService;

        [Input]
        public SendEmailToHerInput Input { get; set; }

        public SendEmailToHerOperation(IMailService mailService)
        {
            _mailService = mailService;
        }

        protected override void OnExecute()
        {
            _mailService.Send(Input.Email, "Hello Miss");
            Console.WriteLine($"Hello Man {Input.Email}!");
        }
    }
}