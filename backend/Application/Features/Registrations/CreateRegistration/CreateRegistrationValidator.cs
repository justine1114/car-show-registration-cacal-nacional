namespace Application.Features.Registrations.CreateRegistration;

using System.Globalization;
using Domain.Entities;
using FluentValidation;

public sealed class CreateRegistrationValidator : AbstractValidator<CreateRegistrationCommand>
{
    public CreateRegistrationValidator()
    {
        RuleFor(x => x.ParticipantName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.ContactNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(@"^09\d{9}$")
            .WithMessage("'Contact Number' must be a Philippine mobile number in the format 09XXXXXXXXX (11 digits).");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.CarMakeModel)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PlateNumber)
            .NotEmpty()
            .MaximumLength(15);

        RuleFor(x => x.RegistrationType)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("'Registration Type' is required (Exhibitor, Competitor or VIP).")
            .IsInEnum()
            .WithMessage("'Registration Type' must be Exhibitor, Competitor or VIP.");

        RuleFor(x => x.AmountPaid)
            .GreaterThanOrEqualTo(0)
            .WithMessage("'Amount Paid' must be 0 or more.")
            .Custom((amountPaid, context) =>
            {
                var type = context.InstanceToValidate.RegistrationType;

                // If the type is missing or invalid, the RegistrationType rule already reports it.
                if (type is null || !Enum.IsDefined(type.Value))
                {
                    return;
                }

                var fee = Registration.GetFeeFor(type.Value);

                if (amountPaid > fee)
                {
                    var feeText = fee.ToString("N0", CultureInfo.InvariantCulture);
                    context.AddFailure($"'Amount Paid' cannot be more than the registration fee ({feeText}).");
                }
            });
    }
}
