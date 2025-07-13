using System;
using System.Collections.Generic;

namespace TrafficViolationFeedbackSystem.Models;

public partial class Violation
{
    public int ViolationId { get; set; }

    public int? ReportId { get; set; }

    public int? ViolatorId { get; set; }

    public decimal? FineAmount { get; set; }

    public DateTime? FineDate { get; set; }

    public bool? PaidStatus { get; set; }

    public virtual ICollection<Appeal> Appeals { get; set; } = new List<Appeal>();

    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();

    public virtual Report? Report { get; set; }

    public virtual User? Violator { get; set; }
}
