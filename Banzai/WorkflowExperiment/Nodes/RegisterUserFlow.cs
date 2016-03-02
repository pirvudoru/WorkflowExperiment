using Banzai;
using WorkflowExperiment.Nodes.Adapters;
using WorkflowExperiment.Nodes.IO;

namespace WorkflowExperiment.Nodes
{
    public class RegisterUserFlow : PipelineNodeBase<RegisterUserContext>
    {
        protected override void OnBeforeExecute(IExecutionContext<RegisterUserContext> context)
        {
            var persistUserNode = NodeFactory.GetNode<PersistUserAdapterNode>();

            var sendEmailNode = new FirstMatchNode<RegisterUserContext>();
            sendEmailNode.AddChild(NodeFactory.GetNode<SendEmailToHerAdapterNode>());
            sendEmailNode.AddChild(NodeFactory.GetNode<SendEmailToHimAdapterNode>());

            var subscribeNode = NodeFactory.GetNode<SubscribeAdapterNode>();

            AddChild(persistUserNode);
            AddChild(sendEmailNode);
            AddChild(subscribeNode);
        }
    }
}