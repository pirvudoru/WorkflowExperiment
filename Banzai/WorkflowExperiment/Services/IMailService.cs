namespace WorkflowExperiment.Services
{
	public interface IMailService
	{
		void Send(string to, string content);
	}
}
