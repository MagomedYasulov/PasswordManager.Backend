using FluentValidation;
using Microsoft.Extensions.Localization;
using PasswordManager.Backend.ViewModels;

namespace PasswordManager.Backend.Validators
{
    public class LoginValidator : AbstractValidator<LoginViewModel>
    {
        public LoginValidator(IStringLocalizer<BaseViewModel> localizer) 
        {
            RuleFor(l => l.Login).NotEmpty();
            RuleFor(l => l.DeviceId).NotEmpty().Guid().WithMessage(localizer["InvalidGuid"]);
            RuleFor(l => l.Password).NotEmpty();
        }
    }
}
