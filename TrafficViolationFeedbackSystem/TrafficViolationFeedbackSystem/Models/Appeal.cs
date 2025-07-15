using System;
using System.Collections.Generic;

namespace TrafficViolationFeedbackSystem.Models;

public partial class Appeal
{
    public int AppealId { get; set; }

    public int? ViolationId { get; set; }

    public int? ViolatorId { get; set; }

    public string? Content { get; set; }

    public DateTime? SubmitDate { get; set; }

    public string? Result { get; set; }

    public virtual Violation? Violation { get; set; }

    public virtual User? Violator { get; set; }
}
