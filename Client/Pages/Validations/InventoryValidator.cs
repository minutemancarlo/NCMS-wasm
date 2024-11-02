using FluentValidation;
using NCMS_wasm.Shared;

namespace NCMS_wasm.Client.Pages.Validations
{
    public class InventoryItemsValidator : AbstractValidator<InventoryItems>
    {
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<InventoryItems>.CreateWithOptions((InventoryItems)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
        public InventoryItemsValidator()
        {
            RuleFor(x => x.ItemId).GreaterThan(0).WithMessage("ItemId must be greater than 0.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty.");
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage("Quantity must be a non-negative number.");
            RuleFor(x => x.Unit).NotEmpty().WithMessage("Unit cannot be empty.");
            RuleFor(x => x.ItemType).IsInEnum().WithMessage("Invalid inventory type.");
        }
    }
}
