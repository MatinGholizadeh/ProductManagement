using FluentValidation;

namespace ProdManagement.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

        RuleFor(x => x.ProductDate)
            .NotEmpty().WithMessage("Product date is required.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Product date cannot be in the future.");

        RuleFor(x => x.ManufacturePhone)
            .NotEmpty().WithMessage("Manufacture phone is required.")
            .Matches(@"^(\+98|0)?9\d{9}$").WithMessage("Phone must be a valid Iranian number.");

        RuleFor(x => x.ManufactureEmail)
            .NotEmpty().WithMessage("Manufacture email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("CreatedBy is required.")
            .Must(c => !string.IsNullOrWhiteSpace(c)).WithMessage("CreatedBy cannot be whitespace.");


        RuleFor(x => x.IsAvailable)
            .NotNull().WithMessage("IsAvailable must be specified.");
    }
}
