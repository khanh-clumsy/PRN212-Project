using System;
using System.Collections.Generic;

namespace TrafficViolationFeedbackSystem.Models;

public partial class ViolationType
{
    public int ViolationTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal StandardFine { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
