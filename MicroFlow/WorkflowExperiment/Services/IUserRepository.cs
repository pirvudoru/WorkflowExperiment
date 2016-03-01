using WorkflowExperiment.Models;

namespace WorkflowExperiment.Services
{
	public interface IUserRepository
	{
		int Save(User user);
	}
}
