using System;
using Moq;
using NFluent;
using NUnit.Framework;
using Overflow;
using WorkflowExperiment.Models;
using WorkflowExperiment.Operations;
using WorkflowExperiment.Operations.IO;
using WorkflowExperiment.Services;

namespace WorkFlowExperimentTests.Operations
{
	[TestFixture]
	public class RegisterUserFlowTests
	{
		private RegisterUserFlow _subject;
		private Mock<IUserRepository> _userRepositoryMock;
		private Mock<IMailService> _mailServiceMock;
		private Mock<ISubscriptionService> _subscriptionServiceMock;
		private User _user;
		private Mock<IWorkflowLogger> _errorLoggerMock;

	    [SetUp]
		public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository> { DefaultValue = DefaultValue.Mock };
            _mailServiceMock = new Mock<IMailService> { DefaultValue = DefaultValue.Mock };
            _subscriptionServiceMock = new Mock<ISubscriptionService> { DefaultValue = DefaultValue.Mock };
            _errorLoggerMock = new Mock<IWorkflowLogger> { DefaultValue = DefaultValue.Mock };

            var simpleOperationResolver = new SimpleOperationResolver();
            simpleOperationResolver.RegisterOperationDependencyInstance(_userRepositoryMock.Object);
            simpleOperationResolver.RegisterOperationDependencyInstance(_mailServiceMock.Object);
            simpleOperationResolver.RegisterOperationDependencyInstance(_subscriptionServiceMock.Object);
            
            _user = new User
            {
                Email = "someoen@example.com",
                Password = "test"
            };

            var configuration = Workflow.Configure<RegisterUserFlow>()
                .WithLogger(_errorLoggerMock.Object)
                .WithResolver(simpleOperationResolver);

            _subject = new RegisterUserFlow
            {
                Input = new RegisterUserInput
                {
                    User = _user
                }
            };
            _subject.Initialize(configuration);
		}

		[Test]
		public void Run_Always_PersistsUser()
		{
            _subject.Execute();

			_userRepositoryMock.Verify(m => m.Save(It.Is<User>(u => u.Email == _user.Email && u.Password == _user.Password)));
		}

		[Test]
		public void Run_WhenUserIdIsEven_SendsEmailToHer()
		{
			_userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(2);

			_subject.Execute();
            
            _mailServiceMock.Verify(m => m.Send(_user.Email, "Hello Miss"));
		}

		[Test]
		public void Run_WhenUserIdIsOdd_SendsEmailToHim()
		{
			_userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(1);

			_subject.Execute();

			_mailServiceMock.Verify(m => m.Send(_user.Email, "Hello Man"));
		}

		[Test]
		public void Run_Always_SubscribesUser()
		{
			_userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(1);

			_subject.Execute();

			_mailServiceMock.Verify(m => m.Send(_user.Email, "Hello Man"));
		}

	    [Test]
	    public void Run_WhenSendEmailFails_TheFlowDoesNotFail()
	    {
	        _subscriptionServiceMock.Setup(m => m.Subscribe(It.IsAny<int>())).Throws(new Exception());

            _subject.Execute();
	    }

	    [Test]
	    public void Run_Always_OutputsUserId()
	    {
	        const int userId = 42;
            _userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(userId);

            _subject.Execute();

	        Check.That(_subject.Output.UserId).Equals(userId);
        }

        [Test]
		public void Run_WhenUserPersistFails_ThrowsError()
		{
			const string errorMessage = "42!!!!";
			var exception = new Exception(errorMessage);
			_userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Throws(exception);

            try
            {
                _subject.Execute();

                Check.That(true).IsFalse();
            }
            catch (Exception e)
            {
                Check.That(e).IsSameReferenceThan(exception);
            }
		}

        [Test]
		public void Run_WhenSendEmailThrowsError_ContinuesLoggingError()
		{
			const string errorMessage = "42!!!!";
			var exception = new Exception(errorMessage);
			_mailServiceMock.Setup(m => m.Send(It.IsAny<string>(), It.IsAny<string>())).Throws(exception);

            _subject.Execute();

            _errorLoggerMock.Verify(m => m.OperationFailed(It.IsAny<IOperation>(), It.IsAny<Exception>()));
		}
	}
}
