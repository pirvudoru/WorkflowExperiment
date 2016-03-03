using Automatonymous;
using WorkflowExperiment.Models;

namespace WorkflowExperiment.Operations.IO
{
    public class RegisterUserContext
    {
        public State CurrentState { get; set; }
        
        public User User { get; set; }

        public int UserId { get; set; }
    }
}