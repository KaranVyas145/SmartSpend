using FluentValidation;
using SmartSpend.Dtos;
using SmartSpend.Enums;

public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

        RuleFor(c => c.TransactionType)
            .NotEmpty().WithMessage("Transaction type is required.")
            .Must(type => type == EnumTransactionType.Income || type == EnumTransactionType.Expense)
            .WithMessage("Transaction type must be either 'Income' or 'Expense'.");

        When(c => !string.IsNullOrEmpty(c.Id), () =>
        {
            RuleFor(c => c.Id)
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("Id must be a valid GUID if specified.");
        });
    }
}
