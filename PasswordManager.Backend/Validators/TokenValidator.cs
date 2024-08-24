using FluentValidation;
using PasswordManager.Backend.ViewModels;

namespace PasswordManager.Backend.Validators
{
    public class TokenValidator : AbstractValidator<TokenViewModel>
    {
        public TokenValidator() 
        {
            RuleFor(t => t.AccessToken).NotEmpty();
            RuleFor(t => t.RefreshToken).NotEmpty();    
        }
    }
}
