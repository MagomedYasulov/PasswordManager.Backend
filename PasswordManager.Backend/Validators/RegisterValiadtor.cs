using FluentValidation;
using Microsoft.Extensions.Localization;
using PasswordManager.Backend.ViewModels;

namespace PasswordManager.Backend.Validators
{
    public class RegisterValiadtor : AbstractValidator<RegisterViewModel>
    {
        public RegisterValiadtor(IStringLocalizer<BaseViewModel> localizer) 
        {
            RuleFor(r => r.Password).NotEmpty().Password().WithMessage(localizer["InvalidPassword"]);
            RuleFor(r => r.Login).NotEmpty().MinimumLength(4).MaximumLength(25);
        }
    }
}
