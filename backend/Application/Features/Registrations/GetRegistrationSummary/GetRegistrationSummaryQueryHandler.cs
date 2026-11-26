namespace Application.Features.Registrations.GetRegistrationSummary;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

public sealed class GetRegistrationSummaryQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<GetRegistrationSummaryQuery, Result<GetRegistrationSummaryResponse>>
{
    public async Task<Result<GetRegistrationSummaryResponse>> HandleAsync(
        GetRegistrationSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        // Load only the three columns we need.
        // SQLite stores decimals as text and cannot add them up (SUM) itself,
        // so the money is added up here in C#.
        var rows = await dbContext.Registrations
            .AsNoTracking()
            .Select(r => new { r.Status, r.RegistrationFee, r.AmountPaid })
            .ToListAsync(cancellationToken);

        // Business rule: cancelled registrations are NOT counted in the money totals
        var activeRows = rows.Where(r => r.Status != RegistrationStatus.Cancelled).ToList();

        var totalFees = activeRows.Sum(r => r.RegistrationFee);
        var totalPayments = activeRows.Sum(r => r.AmountPaid);

        var response = new GetRegistrationSummaryResponse(
            TotalRegistrations: rows.Count,
            Confirmed: rows.Count(r => r.Status == RegistrationStatus.Confirmed),
            CheckedIn: rows.Count(r => r.Status == RegistrationStatus.CheckedIn),
            Pending: rows.Count(r => r.Status == RegistrationStatus.Pending),
            Cancelled: rows.Count(r => r.Status == RegistrationStatus.Cancelled),
            TotalFees: totalFees,
            TotalPayments: totalPayments,
            UncollectedPayments: totalFees - totalPayments);

        return Result<GetRegistrationSummaryResponse>.Success(response);
    }
}
