using System;
using System.Collections.Generic;

namespace TrafficViolationFeedbackSystem.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? ResetCode { get; set; }

    public DateTime? ResetCodeExpiry { get; set; }

    public virtual ICollection<Appeal> Appeals { get; set; } = new List<Appeal>();

    public virtual ICollection<Fine> FineConfirmedByNavigations { get; set; } = new List<Fine>();

    public virtual ICollection<Fine> FineCreatedByNavigations { get; set; } = new List<Fine>();

    public virtual ICollection<Report> ReportProcessedByNavigations { get; set; } = new List<Report>();

    public virtual ICollection<Report> ReportReporters { get; set; } = new List<Report>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public virtual ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
