using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficViolationFeedbackSystem.ViewModels
{
    public class ReportViewModel
    {
        public int ReportId { get; set; }
        public int ReportedId { get; set; }
        public string PlateNumber { get; set; }
        public string ViolationTypeName { get; set; }
        public string OwnerName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime? ReportDate { get; set; }
        public string Status { get; set; }
        public string StatusDisplay => Status switch
        {
            "Pending" => "Chờ duyệt",
            "Approved" => "Đã duyệt",
            "Rejected" => "Từ chối",
            _ => "Không xác định"
        };
        public bool IsActionVisible => Status == "Pending";
    }
}
