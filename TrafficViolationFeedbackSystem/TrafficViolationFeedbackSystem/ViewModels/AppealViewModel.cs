using System;
using ControlzEx.Standard;

namespace TrafficViolationFeedbackSystem.ViewModels
{
    public class AppealViewModel
    {
        public int AppealId { get; set; }
        public int ViolationId { get; set; }
        public string PlateNumber { get; set; }
        public string ViolatorName { get; set; }
        public string Content { get; set; }
        public DateTime? SubmitDate { get; set; }
        public string Result { get; set; }

        public string DisplayResult
        {
            get
            {
                return Result switch
                {
                    "Approved" => "Đã duyệt",
                    "Rejected" => "Từ chối",
                    "Pending" => "Đang chờ",
                    _ => Result
                };
            }
        }
        public bool IsActionVisible => Result == "Pending";

    }
} 