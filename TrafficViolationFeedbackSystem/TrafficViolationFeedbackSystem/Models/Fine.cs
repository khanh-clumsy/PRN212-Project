using System;
using System.Collections.Generic;

namespace TrafficViolationFeedbackSystem.Models;

public partial class Fine
{
    public int FineId { get; set; }

    public int ViolationId { get; set; }

    public string? PaymentMethod { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal? Amount { get; set; }

    public bool? IsConfirmed { get; set; }

    public virtual Violation Violation { get; set; } = null!;
}
