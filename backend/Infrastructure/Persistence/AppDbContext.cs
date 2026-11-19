namespace Infrastructure.Persistence;

using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(20);

            // SQLite does not remember that a date is UTC, so it comes back as "unspecified".
            // This tells EF Core to mark the date as UTC when it is read, so JSON ends with "Z".
            entity.Property(e => e.RegistrationDate)
                .IsRequired()
                .HasConversion(
                    date => date,
                    date => DateTime.SpecifyKind(date, DateTimeKind.Utc));

            entity.Property(e => e.ParticipantName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.ContactNumber).IsRequired().HasMaxLength(11);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(300);
            entity.Property(e => e.CarMakeModel).IsRequired().HasMaxLength(100);

            // NOCASE makes "abc 1234" and "ABC 1234" count as the same plate (one car = one registration)
            entity.Property(e => e.PlateNumber).IsRequired().HasMaxLength(15).UseCollation("NOCASE");

            // Enums are stored as text ("Competitor", "Pending") so they are readable in DB Browser
            entity.Property(e => e.RegistrationType).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

            entity.Property(e => e.RegistrationFee).HasPrecision(18, 2);
            entity.Property(e => e.AmountPaid).HasPrecision(18, 2);

            // Unique indexes
            entity.HasIndex(e => e.RegistrationNumber).IsUnique();
            entity.HasIndex(e => e.PlateNumber).IsUnique();
        });
    }
}
