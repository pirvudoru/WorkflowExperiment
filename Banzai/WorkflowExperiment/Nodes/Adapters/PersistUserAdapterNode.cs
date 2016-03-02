using System.Threading.Tasks;
using Banzai;
using WorkflowExperiment.Nodes.IO;

namespace WorkflowExperiment.Nodes.Adapters
{
    public class PersistUserAdapterNode : TransitionNode<RegisterUserContext, PersistUserContext>
    {
        protected override Task<PersistUserContext> TransitionSourceAsync(
            IExecutionContext<RegisterUserContext> sourceContext)
        {
            return Task.FromResult(new PersistUserContext
            {
                User = sourceContext.Subject.User
            });
        }

        protected override Task<RegisterUserContext> TransitionResultAsync(
            IExecutionContext<RegisterUserContext> sourceContext, NodeResult result)
        {
            var registerUserContext = sourceContext.Subject;

            registerUserContext.UserId = result.GetSubjectAs<PersistUserContext>().Id;

            return Task.FromResult(registerUserContext);
        }

        protected override void OnBeforeExecute(IExecutionContext<RegisterUserContext> context)
        {
            ChildNode = NodeFactory.GetNode<PersistUserNode>();
        }
    }
}