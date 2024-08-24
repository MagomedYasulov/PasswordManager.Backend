using FluentValidation;
using System.Text.RegularExpressions;

namespace PasswordManager.Backend.Validators
{
    public static partial class ValidatorExtentions
    {
        public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilderOptions<T, string> ruleBuilder)
        {
            var regex = PasswordRegex();
            return ruleBuilder.Matches(regex);
        }

        public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var regex = PasswordRegex();
            return ruleBuilder.Matches(regex);
        }

        public static IRuleBuilderOptions<T, string> Guid<T>(this IRuleBuilderOptions<T, string> ruleBuilder)
        {
            var regex = GuidRegex();
            return ruleBuilder.Matches(regex);
        }

        public static IRuleBuilderOptions<T, string> Guid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var regex = GuidRegex();
            return ruleBuilder.Matches(regex);
        }

        [GeneratedRegex(@"(?im)^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$")]
        public static partial Regex GuidRegex();

        [GeneratedRegex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$")]
        public static partial Regex PasswordRegex();
    }
}
