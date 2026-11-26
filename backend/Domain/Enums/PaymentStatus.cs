namespace Domain.Enums;

// Computed from AmountPaid and RegistrationFee. It is NOT stored in the database.
public enum PaymentStatus
{
    Unpaid,
    Partial,
    Paid
}
