using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.Billing.Domain.Entities;

/// <summary>
/// Invoice status enumeration
/// </summary>
public sealed class InvoiceStatus : Enumeration<InvoiceStatus>
{
    public static readonly InvoiceStatus Draft = new(1, nameof(Draft));
    public static readonly InvoiceStatus Pending = new(2, nameof(Pending));
    public static readonly InvoiceStatus Paid = new(3, nameof(Paid));
    public static readonly InvoiceStatus PartiallyPaid = new(4, nameof(PartiallyPaid));
    public static readonly InvoiceStatus Overdue = new(5, nameof(Overdue));
    public static readonly InvoiceStatus Cancelled = new(6, nameof(Cancelled));
    public static readonly InvoiceStatus Refunded = new(7, nameof(Refunded));

    private InvoiceStatus(int value, string name) : base(value, name) { }
}

/// <summary>
/// Invoice aggregate root for billing and settlement
/// </summary>
public sealed class Invoice : AggregateRoot<Guid>
{
    private readonly List<InvoiceLineItem> _lineItems = [];

    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid BookingId { get; private set; }
    public Guid CustomerId { get; private set; }
    public InvoiceStatus Status { get; private set; } = null!;
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public DateTime IssuedAt { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? PaidAt { get; private set; }

    public IReadOnlyList<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();
    public decimal BalanceDue => TotalAmount - PaidAmount;

    private Invoice() { }

    public static Invoice Create(Guid bookingId, Guid customerId, DateTime dueDate)
    {
        return new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = GenerateInvoiceNumber(),
            BookingId = bookingId,
            CustomerId = customerId,
            Status = InvoiceStatus.Draft,
            IssuedAt = DateTime.UtcNow,
            DueDate = dueDate,
            SubTotal = 0,
            TaxAmount = 0,
            TotalAmount = 0,
            PaidAmount = 0
        };
    }

    public void AddLineItem(string description, decimal quantity, decimal unitPrice)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft invoice.");

        var lineItem = new InvoiceLineItem(description, quantity, unitPrice);
        _lineItems.Add(lineItem);
        RecalculateTotals();
    }

    public void FinalizeInvoice()
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Can only finalize draft invoices.");

        if (!_lineItems.Any())
            throw new InvalidOperationException("Invoice must have at least one line item.");

        Status = InvoiceStatus.Pending;
    }

    public void RecordPayment(decimal amount)
    {
        if (Status != InvoiceStatus.Pending && Status != InvoiceStatus.PartiallyPaid && Status != InvoiceStatus.Overdue)
            throw new InvalidOperationException("Cannot record payment for this invoice.");

        PaidAmount += amount;

        if (PaidAmount >= TotalAmount)
        {
            Status = InvoiceStatus.Paid;
            PaidAt = DateTime.UtcNow;
        }
        else
        {
            Status = InvoiceStatus.PartiallyPaid;
        }
    }

    public void MarkAsOverdue()
    {
        if (Status == InvoiceStatus.Pending && DateTime.UtcNow > DueDate)
        {
            Status = InvoiceStatus.Overdue;
        }
    }

    public void Refund()
    {
        if (Status != InvoiceStatus.Paid && Status != InvoiceStatus.PartiallyPaid)
            throw new InvalidOperationException("Can only refund paid invoices.");

        Status = InvoiceStatus.Refunded;
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.Refunded)
            throw new InvalidOperationException("Cannot cancel paid or refunded invoices.");

        Status = InvoiceStatus.Cancelled;
    }

    private void RecalculateTotals()
    {
        SubTotal = _lineItems.Sum(li => li.Amount);
        TaxAmount = SubTotal * 0.1m; // 10% tax rate
        TotalAmount = SubTotal + TaxAmount;
    }

    private static string GenerateInvoiceNumber()
    {
        return $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpperInvariant()}";
    }
}

/// <summary>
/// Invoice line item value object
/// </summary>
public sealed class InvoiceLineItem : ValueObject
{
    public string Description { get; }
    public decimal Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal Amount => Quantity * UnitPrice;

    public InvoiceLineItem(string description, decimal quantity, decimal unitPrice)
    {
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Description;
        yield return Quantity;
        yield return UnitPrice;
    }
}
