using FluentValidation;
using NCMS_wasm.Shared;
using System.Text.RegularExpressions;

namespace NCMS_wasm.Client.Pages.Validations
{
    public class CreateUserValidator : AbstractValidator<UserInfo>
    {
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserInfo>.CreateWithOptions((UserInfo)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
        public CreateUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email Address is required.")
                .EmailAddress().WithMessage("Invalid Email Address format.");
            RuleFor(x => x.GivenName)
                .NotEmpty().WithMessage("First Name is required.");
            RuleFor(x => x.FamilyName)
                .NotEmpty().WithMessage("Last Name is required.");
            RuleFor(x => x.Password)
              .Cascade(CascadeMode.StopOnFirstFailure)
              .NotEmpty().WithMessage("Password is required.")
              .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
              .Must(ContainCapitalLetter).WithMessage("Password must contain at least one capital letter.")
              .Must(ContainLowerCaseLetter).WithMessage("Password must contain at least one lowercase letter.")
              .Must(ContainDigit).WithMessage("Password must contain at least one digit.")
               .Must(ContainSymbol).WithMessage("Password must contain at least one symbol (non-alphanumeric character).");
        }
        
        private bool ContainCapitalLetter(string password)
        {
            return password.Any(char.IsUpper);
        }

        private bool ContainLowerCaseLetter(string password)
        {
            return password.Any(char.IsLower);
        }

        private bool ContainDigit(string password)
        {
            return password.Any(char.IsDigit);
        }

        private bool ContainSymbol(string password)
        {
            // Define symbols as characters other than letters or digits
            var symbols = new[] { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '{', '}', '[', ']', '|', '\\', ';', ':', '\'', '"', '<', '>', ',', '.', '/', '?' };

            return password.Any(c => symbols.Contains(c));
        }

    }    
}
