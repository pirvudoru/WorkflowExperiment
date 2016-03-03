using System;
using System.Threading.Tasks;
using Automatonymous;
using Moq;
using NFluent;
using NUnit.Framework;
using WorkflowExperiment.Models;
using WorkflowExperiment.Operations;
using WorkflowExperiment.Operations.IO;
using WorkflowExperiment.Services;

namespace WorkFlowExperimentTests.Operations
{
    [TestFixture]
    public class RegisterUserFlowTests
    {
        private RegisterUserStateMachine _subject;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMailService> _mailServiceMock;
        private Mock<ISubscriptionService> _subscriptionServiceMock;
        private RegisterUserContext _context;
        private Mock<IErrorLogger> _errorLoggerMock;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository> { DefaultValue = DefaultValue.Mock };
            _mailServiceMock = new Mock<IMailService> { DefaultValue = DefaultValue.Mock };
            _subscriptionServiceMock = new Mock<ISubscriptionService> { DefaultValue = DefaultValue.Mock };
            _errorLoggerMock = new Mock<IErrorLogger> { DefaultValue = DefaultValue.Mock };

            _context = new RegisterUserContext
            {
                User = new User
                {
                    Email = "someoen@example.com",
                    Password = "test"
                }
            };

            _subject = new RegisterUserStateMachine(_userRepositoryMock.Object, _mailServiceMock.Object, _subscriptionServiceMock.Object, _errorLoggerMock.Object);
        }

        [Test]
        public async Task Run_Always_PersistsUser()
        {
            await _subject.RaiseEvent(_context, _subject.Execute);

            _userRepositoryMock.Verify(m => m.Save(It.Is<User>(u => u.Email == _context.User.Email && u.Password == _context.User.Password)));
        }

        [Test]
        public async Task Run_WhenUserIdIsEven_SendsEmailToHer()
        {
            _userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(2);

            await _subject.RaiseEvent(_context, _subject.Execute);

            _mailServiceMock.Verify(m => m.Send(_context.User.Email, "Hello Miss"));
        }

        [Test]
        public async Task Run_WhenUserIdIsOdd_SendsEmailToHim()
        {
            _userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(1);

            await _subject.RaiseEvent(_context, _subject.Execute);

            _mailServiceMock.Verify(m => m.Send(_context.User.Email, "Hello Man"));
        }

        [Test]
        public async Task Run_Always_SubscribesUser()
        {
            _userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(1);

            await _subject.RaiseEvent(_context, _subject.Execute);

            _mailServiceMock.Verify(m => m.Send(_context.User.Email, "Hello Man"));
        }

        [Test]
        public async Task Run_WhenSendEmailFails_TheFlowDoesNotFail()
        {
            _subscriptionServiceMock.Setup(m => m.Subscribe(It.IsAny<int>())).Throws(new Exception());

            await _subject.RaiseEvent(_context, _subject.Execute);
        }

        [Test]
        public async Task Run_WhenSendEmailFails_LogsTheException()
        {
            var exception = new Exception();
            _subscriptionServiceMock.Setup(m => m.Subscribe(It.IsAny<int>())).Throws(exception);

            await _subject.RaiseEvent(_context, _subject.Execute);

            _errorLoggerMock.Verify(m => m.Exception(exception));
        }

        [Test]
        public async Task Run_Always_OutputsUserId()
        {
            const int userId = 42;
            _userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(userId);

            await _subject.RaiseEvent(_context, _subject.Execute);

            Check.That(_context.UserId).Equals(userId);
        }
    }
}
