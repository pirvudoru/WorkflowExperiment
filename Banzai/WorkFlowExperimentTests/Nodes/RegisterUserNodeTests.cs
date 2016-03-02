using System;
using System.Threading.Tasks;
using Autofac;
using Banzai.Autofac;
using Banzai.Logging;
using Moq;
using NFluent;
using NUnit.Framework;
using WorkflowExperiment.Models;
using WorkflowExperiment.Nodes;
using WorkflowExperiment.Nodes.IO;
using WorkflowExperiment.Services;

namespace WorkFlowExperimentTests.Nodes
{
    [TestFixture]
    public class RegisterUserNodeTests
    {
        private RegisterUserFlow _subject;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMailService> _mailServiceMock;
        private Mock<ISubscriptionService> _subscriptionServiceMock;
        private User _user;
        private Mock<ILogWriter> _logWriterMock;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository> {DefaultValue = DefaultValue.Mock};
            _mailServiceMock = new Mock<IMailService> {DefaultValue = DefaultValue.Mock};
            _subscriptionServiceMock = new Mock<ISubscriptionService> {DefaultValue = DefaultValue.Mock};
            _logWriterMock = new Mock<ILogWriter> {DefaultValue = DefaultValue.Mock};

            _user = new User
            {
                Email = "someoen@example.com",
                Password = "test"
            };

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(_userRepositoryMock.Object);
            containerBuilder.RegisterInstance(_mailServiceMock.Object);
            containerBuilder.RegisterInstance(_subscriptionServiceMock.Object);
            containerBuilder.RegisterBanzaiNodes<RegisterUserFlow>(true);

            var logWriterFactoryMock = new Mock<ILogWriterFactory>();
            logWriterFactoryMock.Setup(m => m.GetLogger(It.IsAny<Type>())).Returns(_logWriterMock.Object);
            LogWriter.SetFactory(logWriterFactoryMock.Object);

            var container = containerBuilder.Build();

            _subject = container.Resolve<RegisterUserFlow>();
        }

        [Test]
        public async Task Run_Always_PersistsUser()
        {
            await _subject.ExecuteAsync(new RegisterUserContext {User = _user});

            _userRepositoryMock.Verify(
                m => m.Save(It.Is<User>(u => u.Email == _user.Email && u.Password == _user.Password)));
        }

        [Test]
        public async Task Run_WhenUserIdIsEven_SendsEmailToHer()
        {
            _userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(2);

            await _subject.ExecuteAsync(new RegisterUserContext {User = _user});

            _mailServiceMock.Verify(m => m.Send(_user.Email, "Hello Miss"));
        }

        [Test]
        public async Task Run_WhenUserIdIsOdd_SendsEmailToHim()
        {
            _userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(1);

            await _subject.ExecuteAsync(new RegisterUserContext {User = _user});

            _mailServiceMock.Verify(m => m.Send(_user.Email, "Hello Man"));
        }

        [Test]
        public async Task Run_Always_SubscribesUser()
        {
            _userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(1);

            await _subject.ExecuteAsync(new RegisterUserContext {User = _user});

            _mailServiceMock.Verify(m => m.Send(_user.Email, "Hello Man"));
        }

        [Test]
        public async Task Run_WhenSendEmailFails_TheFlowDoesNotFail()
        {
            _subscriptionServiceMock.Setup(m => m.Subscribe(It.IsAny<int>())).Throws(new Exception());

            await _subject.ExecuteAsync(new RegisterUserContext {User = _user});
        }

        [Test]
        public async Task Run_Always_OutputsUserId()
        {
            const int userId = 42;
            _userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Returns(userId);

            var result = await _subject.ExecuteAsync(new RegisterUserContext {User = _user});

            Check.That(result.GetSubjectAs<RegisterUserContext>().UserId).Equals(userId);
        }

        [Test]
        public async Task Run_WhenUserPersistFails_ThrowsError()
        {
            const string errorMessage = "42!!!!";
            var exception = new Exception(errorMessage);
            _userRepositoryMock.Setup(m => m.Save(It.IsAny<User>())).Throws(exception);

            var result = await _subject.ExecuteAsync(new RegisterUserContext {User = _user});

            Check.That(result.Exception).IsSameReferenceThan(exception);
        }

        [Test]
        public async Task Run_WhenSendEmailThrowsError_ContinuesLoggingError()
        {
            const string errorMessage = "42!!!!";
            var exception = new Exception(errorMessage);
            _mailServiceMock.Setup(m => m.Send(It.IsAny<string>(), It.IsAny<string>())).Throws(exception);

            await _subject.ExecuteAsync(new RegisterUserContext {User = _user});

            _logWriterMock.Verify(m => m.Error(It.IsAny<string>(), exception));
        }
    }
}