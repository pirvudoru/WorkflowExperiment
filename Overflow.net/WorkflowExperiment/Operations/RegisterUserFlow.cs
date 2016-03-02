using System.Collections.Generic;
using Overflow;
using WorkflowExperiment.Operations.IO;

namespace WorkflowExperiment.Operations
{
    public class RegisterUserFlow : Operation
    {
        [Input]
        public RegisterUserInput Input { get; set; }

        [Output]
        public RegisterUserOutput Output { get; set; }

        public override IEnumerable<IOperation> GetChildOperations()
        {
            yield return Create<PersistUserOperation, PersistUserInput>(new PersistUserInput { User = Input.User });
            var persistUserOutput = GetChildOutputValue<PersistUserOutput>();
            if (persistUserOutput.Id % 2 == 0)
            {
                yield return Create<SendEmailToHerOperation, SendEmailToHerInput>(new SendEmailToHerInput
                {
                    Email = Input.User.Email
                });
            }
            else
            {
                yield return Create<SendEmailToHimOperation, SendEmailToHimInput>(new SendEmailToHimInput
                {
                    Email = Input.User.Email
                });
            }

            yield return Create<SubscribeOperation, SubscribeInput>(new SubscribeInput { UserId = persistUserOutput.Id });

            Output = new RegisterUserOutput
            {
                UserId = persistUserOutput.Id
            };
        }
    }
}