namespace Infrastructure.Persistence;

using Domain.Entities;
using Domain.Enums;

// Adds 25 sample registrations when the Registrations table is empty.
//
// What the sample data covers:
//   Types:    6 Exhibitor, 11 Competitor, 5 VIP (22 active) + 3 cancelled
//   Status:   10 Confirmed, 6 CheckedIn, 6 Pending, 3 Cancelled
//   Payments: Unpaid, Partial and Paid
//
// Summary of the sample data (cancelled registrations are excluded from the money):
//   totalRegistrations 25, totalFees 61500, totalPayments 42000, uncollectedPayments 19500
public static class RegistrationSeeder
{
    private sealed record SeedRow(
        string Name,
        string Car,
        RegistrationType Type,
        RegistrationStatus Status,
        decimal AmountPaid);

    private static readonly string[] Addresses =
    [
        "123 Rizal St., Sorsogon City",
        "45 Magsaysay Ave., Sorsogon City",
        "78 Burgos St., Bacon District, Sorsogon City",
        "12 Rizal Road, Gubat, Sorsogon",
        "9 Mabini St., Casiguran, Sorsogon",
        "56 Quezon Ave., Naga City",
        "34 Panganiban Drive, Naga City",
        "210 Magsaysay Ave., Legazpi City",
        "88 Peralta St., Irosin, Sorsogon",
        "17 Bonifacio St., Donsol, Sorsogon"
    ];

    // Row 1 is the oldest registration, row 25 is the newest
    private static readonly SeedRow[] Rows =
    [
        new("Maria Clara Santos", "Honda Civic EK9", RegistrationType.Competitor, RegistrationStatus.CheckedIn, 2500m),
        new("Juan Carlo Reyes", "Nissan Skyline GT-R R34", RegistrationType.VIP, RegistrationStatus.CheckedIn, 5000m),
        new("Ana Marie Villanueva", "Mazda RX-7 FD", RegistrationType.Competitor, RegistrationStatus.CheckedIn, 2500m),
        new("Pedro Miguel Dela Cruz", "Toyota AE86 Trueno", RegistrationType.Exhibitor, RegistrationStatus.CheckedIn, 1500m),
        new("Rosa Mae Navarro", "Porsche 911 Carrera", RegistrationType.VIP, RegistrationStatus.CheckedIn, 5000m),
        new("Carlo Jose Mendoza", "Subaru Impreza WRX STI", RegistrationType.Competitor, RegistrationStatus.CheckedIn, 2500m),
        new("Liza Mae Bautista", "Mercedes-Benz C63 AMG", RegistrationType.VIP, RegistrationStatus.Confirmed, 5000m),
        new("Mark Anthony Garcia", "BMW M3 E46", RegistrationType.VIP, RegistrationStatus.Confirmed, 2500m),
        new("Joanna Lynn Ramos", "Mitsubishi Lancer Evolution IX", RegistrationType.Competitor, RegistrationStatus.Confirmed, 2500m),
        new("Ricardo Luis Fernandez", "Honda S2000", RegistrationType.Competitor, RegistrationStatus.Confirmed, 2500m),
        new("Angela Rose Torres", "Mazda MX-5 NA", RegistrationType.Competitor, RegistrationStatus.Confirmed, 1000m),
        new("Jun Michael Castillo", "Nissan 370Z", RegistrationType.Competitor, RegistrationStatus.Confirmed, 2500m),
        new("Kristine Joy Aquino", "Toyota GR86", RegistrationType.Exhibitor, RegistrationStatus.Confirmed, 1500m),
        new("Dennis Paul Lim", "Ford Mustang GT", RegistrationType.Exhibitor, RegistrationStatus.Confirmed, 1500m),
        new("Teresa Faith Gomez", "Chevrolet Camaro SS", RegistrationType.Exhibitor, RegistrationStatus.Confirmed, 500m),
        new("Albert John Morales", "Toyota Supra MK4", RegistrationType.Competitor, RegistrationStatus.Confirmed, 2000m),
        new("Sofia Grace Domingo", "Lexus LC 500", RegistrationType.VIP, RegistrationStatus.Pending, 0m),
        new("Marco Antonio Pascual", "Nissan Silvia S15", RegistrationType.Competitor, RegistrationStatus.Pending, 0m),
        new("Bea Camille Salazar", "Honda Integra Type R", RegistrationType.Competitor, RegistrationStatus.Pending, 0m),
        new("Jose Ramon Cabrera", "Toyota Chaser JZX100", RegistrationType.Competitor, RegistrationStatus.Pending, 1000m),
        new("Nina Patricia Flores", "Volkswagen Golf GTI", RegistrationType.Exhibitor, RegistrationStatus.Pending, 0m),
        new("Julian Andres Soriano", "Mini Cooper S", RegistrationType.Exhibitor, RegistrationStatus.Pending, 500m),
        new("Elena Marie Valdez", "Suzuki Jimny", RegistrationType.Exhibitor, RegistrationStatus.Cancelled, 1500m),
        new("Francis Noel Aguilar", "Mazda 3 Speed", RegistrationType.Competitor, RegistrationStatus.Cancelled, 0m),
        new("Camille Anne Ocampo", "Audi R8", RegistrationType.VIP, RegistrationStatus.Cancelled, 2000m)
    ];

    public static void Seed(AppDbContext dbContext)
    {
        // Seed only when the table is empty, so restarting the API never creates duplicates
        if (dbContext.Registrations.Any())
        {
            return;
        }

        // All dates are in the past (UTC), so registrations created later always appear first in the list
        var firstDate = new DateTime(2026, 10, 1, 8, 30, 0, DateTimeKind.Utc);
        var registrations = new List<Registration>();

        for (var index = 0; index < Rows.Length; index++)
        {
            var row = Rows[index];
            var number = index + 1;

            var nameParts = row.Name.Split(' ');
            var email = $"{nameParts[0]}.{nameParts[^1]}@example.com".ToLowerInvariant();

            registrations.Add(new Registration
            {
                // Fixed IDs, so the same registration has the same ID after every reset
                Id = Guid.Parse($"5eed0000-0000-0000-0000-{number:D12}"),
                RegistrationNumber = $"CS27-{number:D4}",
                RegistrationDate = firstDate.AddDays(number).AddMinutes(number * 37),
                ParticipantName = row.Name,
                ContactNumber = $"09{171000000 + number}",
                Email = email,
                Address = Addresses[index % Addresses.Length],
                CarMakeModel = row.Car,
                PlateNumber = $"CSW {1000 + number}",
                RegistrationType = row.Type,
                Status = row.Status,
                RegistrationFee = Registration.GetFeeFor(row.Type),
                AmountPaid = row.AmountPaid
            });
        }

        dbContext.Registrations.AddRange(registrations);
        dbContext.SaveChanges();
    }
}
