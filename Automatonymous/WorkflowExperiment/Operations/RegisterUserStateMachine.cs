using System;
using Automatonymous;
using WorkflowExperiment.Operations.IO;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Operations
{
    public class RegisterUserStateMachine : AutomatonymousStateMachine<RegisterUserContext>
    {
        public Event Execute { get; set; }

        protected Event PersistedUser { get; set; }
        protected Event SentEmailToHer { get; set; }
        protected Event SentEmailToHim { get; set; }
        protected Event Subscribed { get; set; }

        public RegisterUserStateMachine(IUserRepository userRepository, IMailService mailService, ISubscriptionService subscriptionService, IErrorLogger logger)
        {
            Initially(
                When(Execute)
                    .Execute(context => new PersistUserActivity(userRepository))
                    .Then(context => context.Raise(PersistedUser)),

                When(PersistedUser)
                    .Execute(context => context.Instance.UserId % 2 == 0 ? (Activity<RegisterUserContext>)new SendEmailToHerOperation(mailService) : new SendEmailToHimOperation(mailService))
                    .Then(context => context.Raise(context.Instance.UserId % 2 == 0 ? SentEmailToHer : SentEmailToHim)),

                When(SentEmailToHer)
                    .Execute(context => new SubscribeOperation(subscriptionService))
                    .Then(context => context.Raise(Subscribed))
                    .Catch<Exception>(binder => binder.Then(context => logger.Exception(context.Exception)))
                    .Finalize(),

                When(SentEmailToHim)
                    .Execute(context => new SubscribeOperation(subscriptionService))
                    .Then(context => context.Raise(Subscribed))
                    .Catch<Exception>(binder => binder.Then(context => logger.Exception(context.Exception)))
                    .Finalize(),

                When(Subscribed)
                    .Finalize()
                );
        }
    }
}