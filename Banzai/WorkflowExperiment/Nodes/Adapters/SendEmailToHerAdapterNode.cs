using System.Threading.Tasks;
using Banzai;
using WorkflowExperiment.Nodes.IO;

namespace WorkflowExperiment.Nodes.Adapters
{
    public class SendEmailToHerAdapterNode : TransitionNode<RegisterUserContext, SendEmailToHerContext>
    {
        public override Task<bool> ShouldExecuteAsync(IExecutionContext<RegisterUserContext> context)
        {
            return Task.FromResult(context.Subject.UserId%2 == 0);
        }

        protected override Task<SendEmailToHerContext> TransitionSourceAsync(
            IExecutionContext<RegisterUserContext> sourceContext)
        {
            return Task.FromResult(new SendEmailToHerContext
            {
                Email = sourceContext.Subject.User.Email
            });
        }

        protected override void OnBeforeExecute(IExecutionContext<RegisterUserContext> context)
        {
            ChildNode = NodeFactory.GetNode<SendEmailToHerNode>();
        }
    }
}