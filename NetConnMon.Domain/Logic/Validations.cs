using FluentValidation;
using NetConnMon.Domain.Entities;

namespace NetConnMon.Domain.Validation
{
    public class EmailSettingsValidator : AbstractValidator<EmailSettings>
    {
        public EmailSettingsValidator()
        {
            RuleFor(x => x.SMTPPassword).MaximumLength(128);
        }
    }
    public class TestDefinitionValidator : AbstractValidator<TestDefinition>
    {
        public TestDefinitionValidator()
        {
            RuleFor(x => x.CheckIntervalSec).Must(GreaterThanZero).WithMessage("Check interval in seconds, > 0");
        }

        private bool GreaterThanZero(ushort value)
        {
            return value > 0;
        }
    }
}
