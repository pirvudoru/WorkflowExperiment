using System;
using MicroFlow;
using WorkflowExperiment.Activities.IO;
using WorkflowExperiment.Models;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Activities
{
	public class PersistUserActivity : SyncActivity<PersistUserResult>
	{
		private readonly IUserRepository _userRepository;

		[Required]
		public User User { get; set; }

		public PersistUserActivity(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		protected override PersistUserResult ExecuteActivity()
		{
			var userId = _userRepository.Save(User);

			User.Id = userId;

			Console.WriteLine($"Persisting user {User.Email} with password {User.Password}");

			return new PersistUserResult
			{
				Id = userId
			};
		}
	}
}