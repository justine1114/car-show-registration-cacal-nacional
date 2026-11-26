namespace Application.Features.Registrations.CreateRegistration;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

public sealed class CreateRegistrationCommandHandler(IAppDbContext dbContext)
    : ICommandHandler<CreateRegistrationCommand, Result<CreateRegistrationResponse>>
{
    private const string NumberPrefix = "CS27-";

    public async Task<Result<CreateRegistrationResponse>> HandleAsync(
        CreateRegistrationCommand command,
        CancellationToken cancellationToken = default)
    {
        var plateNumber = command.PlateNumber.Trim();

        // Business rule: one car = one registration.
        // The PlateNumber column uses NOCASE, so "abc 1234" and "ABC 1234" are the same plate.
        var plateAlreadyUsed = await dbContext.Registrations
            .AnyAsync(r => r.PlateNumber == plateNumber, cancellationToken);

        if (plateAlreadyUsed)
        {
            return Result<CreateRegistrationResponse>.Failure(
                Error.Conflict(
                    "Registration.Conflict",
                    $"A registration with plate number '{plateNumber}' already exists."));
        }

        // The validator already made sure RegistrationType has a valid value.
        var registrationType = command.RegistrationType!.Value;

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = await GenerateNextRegistrationNumberAsync(cancellationToken),
            RegistrationDate = DateTime.UtcNow,
            ParticipantName = command.ParticipantName.Trim(),
            ContactNumber = command.ContactNumber.Trim(),
            Email = command.Email.Trim(),
            Address = command.Address.Trim(),
            CarMakeModel = command.CarMakeModel.Trim(),
            PlateNumber = plateNumber,
            RegistrationType = registrationType,
            Status = RegistrationStatus.Pending,
            RegistrationFee = Registration.GetFeeFor(registrationType),
            AmountPaid = command.AmountPaid
        };

        dbContext.Registrations.Add(registration);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new CreateRegistrationResponse(
            registration.Id,
            registration.RegistrationNumber,
            registration.RegistrationDate,
            registration.ParticipantName,
            registration.ContactNumber,
            registration.Email,
            registration.Address,
            registration.CarMakeModel,
            registration.PlateNumber,
            registration.RegistrationType,
            registration.Status,
            registration.RegistrationFee,
            registration.AmountPaid,
            registration.GetPaymentStatus());

        return Result<CreateRegistrationResponse>.Success(response);
    }

    // Finds the highest existing number (for example CS27-0025) and adds 1.
    // The numbers are zero-padded, so sorting them as text also sorts them by value.
    private async Task<string> GenerateNextRegistrationNumberAsync(CancellationToken cancellationToken)
    {
        var lastNumber = await dbContext.Registrations
            .OrderByDescending(r => r.RegistrationNumber)
            .Select(r => r.RegistrationNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = 1;

        if (lastNumber is not null
            && lastNumber.StartsWith(NumberPrefix)
            && int.TryParse(lastNumber[NumberPrefix.Length..], out var lastSequence))
        {
            nextSequence = lastSequence + 1;
        }

        return $"{NumberPrefix}{nextSequence:D4}";
    }
}
