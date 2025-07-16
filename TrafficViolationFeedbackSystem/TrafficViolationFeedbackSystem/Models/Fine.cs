using System;
using System.Collections.Generic;

namespace TrafficViolationFeedbackSystem.Models;

public partial class Fine
{
    public int FineId { get; set; }

    public int ViolationId { get; set; }

    public string? PaymentMethod { get; set; }

    public string? TransactionCode { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal? Amount { get; set; }

    public string? Status { get; set; }

    public int? CreatedBy { get; set; }

    public int? ConfirmedBy { get; set; }

    public virtual User? ConfirmedByNavigation { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Violation Violation { get; set; } = null!;
}
