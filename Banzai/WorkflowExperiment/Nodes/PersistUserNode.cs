using System;
using System.Threading.Tasks;
using Banzai;
using WorkflowExperiment.Nodes.IO;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Nodes
{
    public class PersistUserNode : Node<PersistUserContext>
    {
        private readonly IUserRepository _userRepository;

        public PersistUserNode(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<PersistUserContext> context)
        {
            var persistUserContext = context.Subject;
            var user = persistUserContext.User;
            persistUserContext.Id = _userRepository.Save(user);

            Console.WriteLine($"Persisting user {user.Email} with password {user.Password}");

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }
}