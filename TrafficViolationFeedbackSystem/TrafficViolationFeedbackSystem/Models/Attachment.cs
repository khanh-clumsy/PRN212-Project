using System;
using System.Collections.Generic;

namespace TrafficViolationFeedbackSystem.Models;

public partial class Attachment
{
    public int AttachmentId { get; set; }

    public int? ReportId { get; set; }

    public string? FilePath { get; set; }

    public string? FileType { get; set; }

    public virtual Report? Report { get; set; }
}
