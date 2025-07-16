using System;
using System.Collections.Generic;

namespace TrafficViolationFeedbackSystem.Models;

public partial class Violation
{
    public int ViolationId { get; set; }

    public int ReportId { get; set; }

    public int ViolatorId { get; set; }

    public decimal? FineAmount { get; set; }

    public DateTime? FineDate { get; set; }

    public DateTime? DueDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Appeal> Appeals { get; set; } = new List<Appeal>();

    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();

    public bool CanAppeal => Appeals == null || Appeals.Count == 0;

    public virtual Report Report { get; set; } = null!;

    public virtual User Violator { get; set; } = null!;
}
