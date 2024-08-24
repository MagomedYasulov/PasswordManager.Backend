using FluentValidation;
using Microsoft.Extensions.Localization;
using PasswordManager.Backend.ViewModels;

namespace PasswordManager.Backend.Validators
{
    public class DeviceValidator : AbstractValidator<DeviceViewModel>
    {
        public DeviceValidator(IStringLocalizer<BaseViewModel> localizer) 
        {
            RuleFor(d => d.DeviceId).NotEmpty().Guid().WithMessage(localizer["InvalidGuid"]);
        }
    }
}
