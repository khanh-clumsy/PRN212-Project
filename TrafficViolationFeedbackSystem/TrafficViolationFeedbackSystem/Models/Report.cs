using System;
using System.Collections.Generic;

namespace TrafficViolationFeedbackSystem.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public int? ReporterId { get; set; }

    public string PlateNumber { get; set; } = null!;

    public int? ViolationTypeId { get; set; }

    public string? Description { get; set; }

    public string? Location { get; set; }

    public DateTime? ReportDate { get; set; }

    public string? Status { get; set; }

    public int? ProcessedBy { get; set; }

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual User? ProcessedByNavigation { get; set; }

    public virtual User? Reporter { get; set; }

    public virtual ViolationType? ViolationType { get; set; }

    public virtual ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
