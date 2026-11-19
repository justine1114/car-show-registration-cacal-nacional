namespace Application.Features.Registrations.GetRegistrations;

using FluentValidation;

public sealed class GetRegistrationsValidator : AbstractValidator<GetRegistrationsQuery>
{
    public GetRegistrationsValidator()
    {
        RuleFor(x => x.ParticipantName)
            .MaximumLength(150);

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("'Type' must be Exhibitor, Competitor or VIP.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("'Status' must be Pending, Confirmed, CheckedIn or Cancelled.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("'Page' must be 1 or more.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("'Page Size' must be between 1 and 50.");
    }
}
