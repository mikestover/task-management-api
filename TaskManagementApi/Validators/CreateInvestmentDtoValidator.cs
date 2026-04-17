using FluentValidation;
using TaskManagementApi.DTOs;

namespace TaskManagementApi.Validators
{
    public class CreateInvestmentDtoValidator : AbstractValidator<CreateInvestmentDto>
    {
        public CreateInvestmentDtoValidator()
        {
            RuleFor(x => x.Abbreviation)
                .NotEmpty().WithMessage("Abbreviation is required")
                .MaximumLength(200).WithMessage("Abbreviation cannot exceed 200 characters");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");

            RuleFor(x => x.Category)
                .MaximumLength(200).WithMessage("Category cannot exceed 200 characters");

            RuleFor(x => x.Shares)
                .GreaterThanOrEqualTo(0).WithMessage("Shares must be non-negative");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero");
        }
    }
}
