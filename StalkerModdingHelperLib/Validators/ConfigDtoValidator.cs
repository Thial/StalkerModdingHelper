using FluentValidation;
using StalkerModdingHelperLib.Model;

namespace StalkerModdingHelperLib.Validators
{
    public class ConfigDtoValidator : AbstractValidator<ConfigDto>
    {
        public ConfigDtoValidator()
        {
            RuleFor(c => c.Instance)
                .SetValidator(new InstanceDtoValidator());

            RuleForEach(c => c.Mods)
                .SetValidator(c => new ModDtoValidator(c));
        }
    }
}