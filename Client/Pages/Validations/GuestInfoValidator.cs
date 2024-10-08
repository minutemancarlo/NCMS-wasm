using FluentValidation;
using NCMS_wasm.Shared;
using System;
namespace NCMS_wasm.Client.Pages.Validations
{   
    public class GuestsInfoValidator : AbstractValidator<GuestsInfo>
    {
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<GuestsInfo>.CreateWithOptions((GuestsInfo)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
        public GuestsInfoValidator()
        {
            // Validate FirstName is not empty and has a length limit
            RuleFor(guest => guest.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(50).WithMessage("First Name cannot be longer than 50 characters.");

            // Validate MiddleName is optional but should have a length limit if provided
            RuleFor(guest => guest.MiddleName)
                .MaximumLength(50).WithMessage("Middle Name cannot be longer than 50 characters.");

            // Validate LastName is not empty and has a length limit
            RuleFor(guest => guest.LastName)
                .NotEmpty().WithMessage("Last Name is required.")
                .MaximumLength(50).WithMessage("Last Name cannot be longer than 50 characters.");

            // Validate Email is not empty and has a valid format
            RuleFor(guest => guest.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            // Validate Phone is not empty and matches a phone number format
            RuleFor(guest => guest.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10,12}$").WithMessage("Phone number must be between 10 to 12 digits.");

            // Validate optional IDType and IDNumber only if IDType is provided
            RuleFor(guest => guest.IDType)
                .NotEmpty().WithMessage("ID Type is required.");                

            RuleFor(guest => guest.IDNumber)
                .NotEmpty().WithMessage("ID Number is required.");
                
            // Validate CheckInDate is required and must be before or on CheckOutDate
            RuleFor(guest => guest.CheckInDate)
                .NotEmpty().WithMessage("Check-in date is required.")
                .LessThanOrEqualTo(guest => guest.CheckOutDate).When(guest => guest.CheckOutDate.HasValue)
                .WithMessage("Check-in date must be before or on the check-out date.");

            
            RuleFor(guest => guest.CheckOutDate)
                .NotEmpty().WithMessage("Check-out date is required.");
                

            RuleFor(guest => guest.ArrivalDate)
                .LessThanOrEqualTo(guest => guest.CheckInDate).When(guest => guest.CheckInDate.HasValue)
                .WithMessage("Arrival date must be on or before the check-in date.");

            
            RuleFor(guest => guest.Rooms)
                .GreaterThan(0).WithMessage("Number of rooms must be at least 1.");

            RuleFor(guest => guest.Children)
                .GreaterThanOrEqualTo(0).WithMessage("Number of children cannot be negative.");

            RuleFor(guest => guest.Adults)
                .GreaterThan(0).WithMessage("There must be at least 1 adult.");
            
            RuleFor(guest => guest.BookingType)
                .IsInEnum().WithMessage("Invalid booking type.");
        }
    }

}
