using MicroFlow;
using WorkflowExperiment.Activities;
using WorkflowExperiment.Activities.IO;
using WorkflowExperiment.Framework;
using WorkflowExperiment.Models;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Flows
{
	public class RegisterUserFlow : Flow
	{
		private readonly IUserRepository _userRepository;
		private readonly IMailService _mailService;
		private readonly ISubscriptionService _subscriptionService;
		private readonly ILogger _logger;

		public override string Name => "Register User";

		[Required]
		public User User { get; set; }

		public RegisterUserFlow(IUserRepository userRepository, IMailService mailService, ISubscriptionService subscriptionService, ILogger logger)
		{
			_userRepository = userRepository;
			_mailService = mailService;
			_subscriptionService = subscriptionService;
			_logger = logger;
		}

		protected override void Build(FlowBuilder builder)
		{
			// Activities
			var persistUserActivity = builder.Activity<PersistUserActivity>();
			var sendEmailToHerActivity = builder.Activity<SendEmailToHerActivity>();
			var sendEmailToHimActivity = builder.Activity<SendEmailToHimActivity>();
			var subscribeActivity = builder.Activity<SubscribeActivity>();
			var sendEmailCondition = builder.Condition("User Id % 2 == 0");

			// Activity Outputs
			var persistUserResult = Result<PersistUserResult>.Of(persistUserActivity);

			// Activity Inputs
			persistUserActivity.Bind(a => a.User).To(User);
			subscribeActivity.Bind(a => a.Id).To(() => persistUserResult.Get().Id);
			sendEmailToHerActivity.Bind(a => a.Email).To(User.Email);
			sendEmailToHimActivity.Bind(a => a.Email).To(User.Email);
			
			// Activity Wirings
			sendEmailCondition.WithCondition(() => persistUserResult.Get().Id % 2 == 0)
				.ConnectTrueTo(sendEmailToHerActivity)
				.ConnectFalseTo(sendEmailToHimActivity);
			persistUserActivity.ConnectTo(sendEmailCondition);
			sendEmailToHerActivity.ConnectTo(subscribeActivity);
			sendEmailToHimActivity.ConnectTo(subscribeActivity);

			// Configuration
			builder.WithInitialNode(persistUserActivity)
				.WithDefaultFaultHandler<DefaultFaultHandler>()
				.WithDefaultCancellationHandler<DefaultCancelationHandler>();
		}

		protected override void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IUserRepository>(_userRepository);
			services.AddSingleton<IMailService>(_mailService);
			services.AddSingleton<ISubscriptionService>(_subscriptionService);
			services.AddSingleton<ILogger>(_logger);

			base.ConfigureServices(services);
		}
	}
}