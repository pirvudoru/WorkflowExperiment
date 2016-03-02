using WorkflowExperiment.Models;

namespace WorkflowExperiment.Nodes.IO
{
    public class RegisterUserContext
    {
        public User User { get; set; }

        public int UserId { get; set; }
    }
}