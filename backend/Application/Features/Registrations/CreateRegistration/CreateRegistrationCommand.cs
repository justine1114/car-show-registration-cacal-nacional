namespace Application.Features.Registrations.CreateRegistration;

using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Enums;

// Only the fields the client is allowed to send.
// RegistrationNumber, RegistrationDate, RegistrationFee and Status are set by the server.
// RegistrationType is nullable so a missing value is reported as a validation error
// instead of silently becoming the first enum value.
public sealed record CreateRegistrationCommand(
    string ParticipantName,
    string ContactNumber,
    string Email,
    string Address,
    string CarMakeModel,
    string PlateNumber,
    RegistrationType? RegistrationType,
    decimal AmountPaid) : ICommand<Result<CreateRegistrationResponse>>;

public sealed record CreateRegistrationResponse(
    Guid Id,
    string RegistrationNumber,
    DateTime RegistrationDate,
    string ParticipantName,
    string ContactNumber,
    string Email,
    string Address,
    string CarMakeModel,
    string PlateNumber,
    RegistrationType RegistrationType,
    RegistrationStatus Status,
    decimal RegistrationFee,
    decimal AmountPaid,
    PaymentStatus PaymentStatus);
