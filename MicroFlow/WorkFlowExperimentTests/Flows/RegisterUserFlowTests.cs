using System;
using System.Threading.Tasks;
using MicroFlow;
using Moq;
using NUnit.Framework;
using WorkflowExperiment.Flows;
using WorkflowExperiment.Models;
using WorkflowExperiment.Services;

namespace WorkFlowExperimentTests.Flows
{
	[TestFixture]
	public class RegisterUserFlowTests
	{
		private RegisterUserFlow _subject;
		private Mock<IUserRepository> _userRepositoryMock;
		private Mock<IMailService> _mailServiceMock;
		private Mock<ISubscriptionService> _subscriptionServiceMock;
		private User _user;
		private Mock<ILogger> _loggerMock;

		[SetUp]
		public void SetUp()
		{
			_userRepositoryMock = new Mock<IUserRepository> { DefaultValue = DefaultValue.Mock };
			_mailServiceMock = new Mock<IMailService> { DefaultValue = DefaultValue.Mock };
			_subscriptionServiceMock = new Mock<ISubscriptionService> { DefaultValue = DefaultValue.Mock };
			_loggerMock = new Mock<ILogger> { DefaultValue = DefaultValue.Mock };

			_user = new User
			{
				Email = "someoen@example.com",
				Password = "test"
			};

			_subject = new RegisterUserFlow(_userRepositoryMock.Object, _mailServiceMock.Object, _subscriptionServiceMock.Object, _loggerMock.Object)
			{
				User = _user
			};
		}

		[Test]
		public async Task Run_Always_PersistsUser()
		{
			await _subject.Run();

			_userRepositoryMock.Verify(m => m.Save(It.Is<User>(u => u.Email == _user.Email && u.Password == _user.Password)));
		}

		[Test]
		public async Task Run_WhenUserIdIsEven_SendsEmailToHer()
		{
			_userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(2);

			await _subject.Run();

			_mailServiceMock.Verify(m => m.Send(_user.Email, "Hello Miss"));
		}

		[Test]
		public async Task Run_WhenUserIdIsOdd_SendsEmailToHim()
		{
			_userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(1);

			await _subject.Run();

			_mailServiceMock.Verify(m => m.Send(_user.Email, "Hello Man"));
		}

		[Test]
		public async Task Run_Always_SubscribesUser()
		{
			_userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(1);

			await _subject.Run();

			_mailServiceMock.Verify(m => m.Send(_user.Email, "Hello Man"));
		}

		[Test]
		public async Task Run_WhenUserPersistFails_LogsError()
		{
			const string errorMessage = "42!!!!";
			var exception = new Exception(errorMessage);
			_userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Throws(exception);

			await _subject.Run();

			_loggerMock.Verify(m => m.Exception(It.Is<AggregateException>(e => e.InnerExceptions.Contains(exception))));
		}
	}
}
