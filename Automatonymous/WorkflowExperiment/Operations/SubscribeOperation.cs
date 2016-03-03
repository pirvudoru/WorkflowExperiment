using System;
using System.Threading.Tasks;
using Automatonymous;
using WorkflowExperiment.Operations.IO;
using WorkflowExperiment.Services;

namespace WorkflowExperiment.Operations
{
    public class SubscribeOperation : Activity<RegisterUserContext>
    {
        private readonly ISubscriptionService _subscriptionService;
        
        public SubscribeOperation(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public Task Execute(BehaviorContext<RegisterUserContext> context, Behavior<RegisterUserContext> next)
        {
            Execute(context.Instance);

            return next.Execute(context);
        }

        public Task Execute<T>(BehaviorContext<RegisterUserContext, T> context, Behavior<RegisterUserContext, T> next)
        {
            Execute(context.Instance);

            return next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<RegisterUserContext, TException> context, Behavior<RegisterUserContext> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<RegisterUserContext, T, TException> context, Behavior<RegisterUserContext, T> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        protected void Execute(RegisterUserContext context)
        {
            _subscriptionService.Subscribe(context.UserId);

            Console.WriteLine($"Subscribing user {context.UserId}");
        }
    }
}