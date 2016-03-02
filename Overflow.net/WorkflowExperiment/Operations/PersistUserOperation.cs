using System;
using Overflow;
using WorkflowExperiment.Operations.IO;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Operations
{
    [Atomic]
	public class PersistUserOperation : Operation
	{
		private readonly IUserRepository _userRepository;
        
        [Input]
        public PersistUserInput Input { get; set; }

        [Output]
        public PersistUserOutput Output { get; set; }

		public PersistUserOperation(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}
        
	    protected override void OnExecute()
	    {
            var userId = _userRepository.Save(Input.User);

            Console.WriteLine($"Persisting user {Input.User.Email} with password {Input.User.Password}");

            Output = new PersistUserOutput
            {
                Id = userId
            };
        }
	}
}