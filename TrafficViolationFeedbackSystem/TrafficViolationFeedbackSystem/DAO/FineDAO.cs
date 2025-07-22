using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.ViewModels;

namespace TrafficViolationFeedbackSystem.DAO
{
    public class FineDAO
    {
        private readonly TrafficViolationFeedbackSystemContext _context;

        public FineDAO()
        {
            _context = new TrafficViolationFeedbackSystemContext();
        }

        public List<FineViewModel> GetPendingFinesByUserId(int userId)
        {
            return _context.Fines
                .Include(f => f.Violation)
                .Where(f => f.Violation.ViolatorId == userId && f.Status == "Pending")
                .Select(f => new FineViewModel
                {
                    FineID = f.FineId,
                    ViolationID = f.ViolationId,
                    ViolationName = f.Violation.Report.ViolationType.Name,
                    Amount = f.Amount,
                    Status = f.Status,
                    PaymentDate = f.PaymentDate
                })
                .ToList();
        }
        public List<FineViewModel> GetAll(int userId)
        {
            return _context.Fines
                .Include(f => f.Violation).ThenInclude(v => v.Report).ThenInclude(r => r.ViolationType)
                .Where(f => f.Violation.ViolatorId == userId)
                .Select(f => new FineViewModel
                {
                    FineID = f.FineId,
                    ViolationID = f.ViolationId,
                    ViolationName = f.Violation.Report.ViolationType.Name,
                    Amount = f.Amount,
                    Status = f.Status,
                    PaymentDate = f.PaymentDate,
                    TransactionCode = f.TransactionCode,
                    PaymentMethod = f.PaymentMethod
                })
                .ToList();
        }
    }

}
