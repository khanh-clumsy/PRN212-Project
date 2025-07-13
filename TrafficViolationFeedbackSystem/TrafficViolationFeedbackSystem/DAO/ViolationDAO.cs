using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.DAO
{
    public class ViolationDAO
    {
        private readonly TrafficViolationFeedbackSystemContext _context;

        public ViolationDAO()
        {
            _context = new TrafficViolationFeedbackSystemContext();
        }

        public List<Violation> GetViolationsForUser(int userId)
        {
            return _context.Violations
                .Include(v => v.Report)
                    .ThenInclude(r => r.ViolationType)
                .Where(v => v.ViolatorId == userId)
                .ToList();
        }


    }
}
