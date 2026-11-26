namespace Application.Features.Registrations.GetRegistrations;

using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Enums;

// Filters and paging come from the query string:
// /api/registrations?participantName=juan&type=VIP&status=Pending&page=1&pageSize=10
public sealed record GetRegistrationsQuery(
    string? ParticipantName,
    RegistrationType? Type,
    RegistrationStatus? Status,
    int Page = 1,
    int PageSize = 10) : IQuery<Result<GetRegistrationsResponse>>;

public sealed record GetRegistrationsResponse(
    IReadOnlyList<RegistrationItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

public sealed record RegistrationItemResponse(
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
