using FluentValidation;
using NCMS_wasm.Shared;
using Nextended.Core.Extensions;

namespace NCMS_wasm.Client.Pages.Validations
{
    public class LoyaltyCardValidator : AbstractValidator<LoyaltyCardInfo>
    {
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<LoyaltyCardInfo>.CreateWithOptions((LoyaltyCardInfo)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
        public LoyaltyCardValidator()
        {
            RuleFor(x => x.CardReference)
                .NotEmpty().WithMessage("RFID Card Reference is Required.");                
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email Address is required.")
                .EmailAddress().WithMessage("Invalid Email Address format.");
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required.");
            RuleFor(x => x.MiddleName)
               .NotEmpty().WithMessage("Middle Name is required.");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{11}$").WithMessage("Phone number must be 11 digits.");

        }
    }
}


