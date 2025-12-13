namespace Ev.Billing.Domain;

public enum InvoiceStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Cancelled = 3
}

public enum PaymentStatus
{
    Initiated = 0,
    Authorized = 1,
    Captured = 2,
    Failed = 3
}
