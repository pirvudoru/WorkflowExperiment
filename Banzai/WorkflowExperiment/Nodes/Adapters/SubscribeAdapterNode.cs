using System.Threading.Tasks;
using Banzai;
using WorkflowExperiment.Nodes.IO;

namespace WorkflowExperiment.Nodes.Adapters
{
    public class SubscribeAdapterNode : TransitionNode<RegisterUserContext, SubscribeContext>
    {
        public override Task<bool> ShouldExecuteAsync(IExecutionContext<RegisterUserContext> context)
        {
            return Task.FromResult(context.Subject.UserId % 2 == 0);
        }

        protected override Task<SubscribeContext> TransitionSourceAsync(
            IExecutionContext<RegisterUserContext> sourceContext)
        {
            return Task.FromResult(new SubscribeContext
            {
                UserId = sourceContext.Subject.UserId
            });
        }

        protected override void OnBeforeExecute(IExecutionContext<RegisterUserContext> context)
        {
            ChildNode = NodeFactory.GetNode<SubscribeNode>();
        }
    }
}