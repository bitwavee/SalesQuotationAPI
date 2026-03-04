using FluentValidation;
using SalesQuotation.Application.Dtos;

namespace SalesQuotation.Application.Validators;

public class CreateEnquiryValidator : AbstractValidator<CreateEnquiryDto>
{
    public CreateEnquiryValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MinimumLength(2).WithMessage("Customer name must be at least 2 characters");

        RuleFor(x => x.CustomerPhone)
            .NotEmpty().WithMessage("Customer phone is required")
            .Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits");

        RuleFor(x => x.CustomerEmail)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.CustomerEmail))
            .WithMessage("Email format is invalid");
    }
}