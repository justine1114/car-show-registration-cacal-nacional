namespace Application.Features.Registrations.GetRegistrations;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

public sealed class GetRegistrationsQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<GetRegistrationsQuery, Result<GetRegistrationsResponse>>
{
    public async Task<Result<GetRegistrationsResponse>> HandleAsync(
        GetRegistrationsQuery query,
        CancellationToken cancellationToken = default)
    {
        // Start with all registrations, then add one Where() for each filter that was sent.
        // Nothing runs in the database until CountAsync / ToListAsync below.
        var registrations = dbContext.Registrations.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.ParticipantName))
        {
            // LIKE in SQLite ignores upper/lower case, and %text% means "contains"
            var pattern = $"%{EscapeLikeText(query.ParticipantName.Trim())}%";
            registrations = registrations.Where(r => EF.Functions.Like(r.ParticipantName, pattern, "\\"));
        }

        if (query.Type is not null)
        {
            var type = query.Type.Value;
            registrations = registrations.Where(r => r.RegistrationType == type);
        }

        if (query.Status is not null)
        {
            var status = query.Status.Value;
            registrations = registrations.Where(r => r.Status == status);
        }

        var totalCount = await registrations.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        // Newest first. RegistrationNumber is a tie-breaker so paging is always stable.
        var skip = (int)Math.Min((long)(query.Page - 1) * query.PageSize, int.MaxValue);

        var pageOfRegistrations = await registrations
            .OrderByDescending(r => r.RegistrationDate)
            .ThenByDescending(r => r.RegistrationNumber)
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        // PaymentStatus is computed in C#, so the rows are mapped after they are loaded.
        var items = pageOfRegistrations
            .Select(r => new RegistrationItemResponse(
                r.Id,
                r.RegistrationNumber,
                r.RegistrationDate,
                r.ParticipantName,
                r.ContactNumber,
                r.Email,
                r.Address,
                r.CarMakeModel,
                r.PlateNumber,
                r.RegistrationType,
                r.Status,
                r.RegistrationFee,
                r.AmountPaid,
                r.GetPaymentStatus()))
            .ToList();

        return Result<GetRegistrationsResponse>.Success(
            new GetRegistrationsResponse(items, query.Page, query.PageSize, totalCount, totalPages));
    }

    // So that a search for "50%" or "a_b" is treated as plain text, not as LIKE wildcards
    private static string EscapeLikeText(string text) =>
        text.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
}
