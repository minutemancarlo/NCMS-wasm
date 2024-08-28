using FluentValidation;
using NCMS_wasm.Shared;

namespace NCMS_wasm.Client.Pages.Validations
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<Employee>.CreateWithOptions((Employee)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
        public EmployeeValidator()
        {
            //RuleFor(e => e.IDNumber)
            //    .NotEmpty().WithMessage("ID Number is required.")
            //    .Length(5, 20).WithMessage("ID Number must be between 5 and 20 characters.");

            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");

            RuleFor(e => e.Address)
              .NotEmpty().WithMessage("Address is required.");

            RuleFor(e => e.PlaceOfBirth)
              .NotEmpty().WithMessage("Place of Birth is required.");

            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(e => e.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{11}$").WithMessage("Phone number must be 11 digits.");

            RuleFor(e => e.SSS)
                .NotEmpty().WithMessage("SSS is required.");

            RuleFor(e => e.PagIbig)
                .NotEmpty().WithMessage("PagIbig is required.");

            RuleFor(e => e.PHIC)
                .NotEmpty().WithMessage("PHIC is required.");                

            RuleFor(e => e.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required.")
                .Must(BeAValidAge).WithMessage("Employee must be at least 18 years old.");

            RuleFor(e => e.Gender)
                .NotEmpty().WithMessage("Gender is required.");

            RuleFor(e => e.Position)
                .NotEmpty().WithMessage("Position is required.");

            RuleFor(e => e.CivilStatus)
                .NotEmpty().WithMessage("Civil Status is required.");

            RuleFor(e => e.Salary)
                .NotEmpty().WithMessage("Salary is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Salary must be a positive number.");

            RuleFor(e => e.DateResigned)
                .NotEmpty().WithMessage("Date Resigned is required.")
                .GreaterThanOrEqualTo(e => e.DateHired)
                .When(e => e.DateResigned.HasValue && e.DateHired.HasValue)
                .WithMessage("Date Resigned must be after Date Hired.");

            RuleFor(e => e.EmergencyContactName)
                .NotEmpty().WithMessage("Emergency Contact Name is required.")
                .MaximumLength(50).WithMessage("Emergency Contact Name cannot exceed 50 characters.");

            RuleFor(e => e.EmergencyContactAddress)
               .NotEmpty().WithMessage("Emergency Contact Address is required.");

            RuleFor(e => e.EmergencyContactRelationship)
               .NotEmpty().WithMessage("Emergency Contact Relationship is required.");

            RuleFor(e => e.EmergencyContactPhone)
                .NotEmpty().WithMessage("Emergency Contact Phone is required.")
                .Matches(@"^\+?\d{11}$").WithMessage("Emergency Contact Phone must be between 11 digits.");

            RuleFor(e => e.BeneficiaryName)
                .NotEmpty().WithMessage("Beneficiary Name is required.")
                .MaximumLength(50).WithMessage("Beneficiary Name cannot exceed 50 characters.");

            RuleFor(e => e.BeneficiaryRelationship)
                .NotEmpty().WithMessage("Beneficiary Relationship is required.")
                .MaximumLength(30).WithMessage("Beneficiary Relationship cannot exceed 30 characters.");

            RuleFor(e => e.BeneficiaryContactInfo)
                .NotEmpty().WithMessage("Beneficiary Contact Info is required.");                

            RuleFor(e => e.DateHired)
               .NotEmpty().WithMessage("Date Hired is required.");
        }

        private bool BeAValidAge(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue) return false;

            int age = DateTime.Today.Year - dateOfBirth.Value.Year;
            if (dateOfBirth.Value.Date > DateTime.Today.AddYears(-age)) age--;

            return age >= 18;
        }
    
}
}
