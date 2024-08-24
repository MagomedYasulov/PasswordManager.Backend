using FluentValidation;
using PasswordManager.Backend.ViewModels;

namespace PasswordManager.Backend.Validators
{
    public class CredentialValidator : AbstractValidator<CredentialViewModel>
    {
        public CredentialValidator() 
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(c => c.Password).NotEmpty();
        }
    }
}
