using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficViolationFeedbackSystem.ViewModels
{
    public class FineViewModel
    {
        public int FineID { get; set; }
        public int ViolationID { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string DisplayResult
        {
            get
            {
                return Status switch
                {
                    "Pending" => "Chờ thanh toán",
                    "Success" => "Đã thanh toán",
                    "Failed" => "Thất bại",
                    "Refunded" => "Đã hoàn tiền",
                    null or "" => "Không xác định",
                    _ => Status
                };
            }
        }
        public string? TransactionCode { get; set; }
        public string? PaymentMethod { get; set; }
        public string ViolationName { get; internal set; }

    }

}
