using System.Threading.Tasks;
using Banzai;
using WorkflowExperiment.Nodes.IO;

namespace WorkflowExperiment.Nodes.Adapters
{
    public class SendEmailToHimAdapterNode : TransitionNode<RegisterUserContext, SendEmailToHimContext>
    {
        public override Task<bool> ShouldExecuteAsync(IExecutionContext<RegisterUserContext> context)
        {
            return Task.FromResult(context.Subject.UserId % 2 == 1);
        }

        protected override Task<SendEmailToHimContext> TransitionSourceAsync(
            IExecutionContext<RegisterUserContext> sourceContext)
        {
            return Task.FromResult(new SendEmailToHimContext
            {
                Email = sourceContext.Subject.User.Email
            });
        }

        protected override void OnBeforeExecute(IExecutionContext<RegisterUserContext> context)
        {
            ChildNode = NodeFactory.GetNode<SendEmailToHimNode>();
        }
    }
}