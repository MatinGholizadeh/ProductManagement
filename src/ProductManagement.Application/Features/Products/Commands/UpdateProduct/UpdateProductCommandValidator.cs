using FluentValidation;

namespace ProdManagement.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        // فقط اگر Name وارد شده باشه، اعتبارسنجی کن
        When(x => x.Name is not null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty when provided.")
                .MaximumLength(100);
        });

        // می‌تونی برای IsAvailable هم شرط بذاری اگر خواستی
    }
}