using System;
using System.Collections.Generic;

namespace TrafficViolationFeedbackSystem.Models;

public partial class AuditLog
{
    public int LogId { get; set; }

    public int? UserId { get; set; }

    public string? Action { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Description { get; set; }

    public virtual User? User { get; set; }
}
