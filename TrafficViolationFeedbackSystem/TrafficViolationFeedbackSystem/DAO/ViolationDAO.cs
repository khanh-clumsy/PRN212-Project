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
                .Include(v => v.Appeals)
                .Where(v => v.ViolatorId == userId)
                .ToList();
        }
        public List<Violation> GetAllViolations()
        {
            return _context.Violations
                .Include(v => v.Report)
                    .ThenInclude(r => r.ViolationType)
                .Include(v => v.Violator)
                .ToList();
        }
        public List<Violation> SearchByDescription(string keyword)
        {
            return _context.Violations
                .Include(v => v.Report)
                    .ThenInclude(r => r.ViolationType)
                .Include(v => v.Violator)
                .Where(v => v.Report.Description.Contains(keyword))
                .ToList();
        }

        public List<Violation> FilterByTypeAndStatus(int? typeId, string status)
        {
            var query = _context.Violations
                .Include(v => v.Report)
                    .ThenInclude(r => r.ViolationType)
                .Include(v => v.Violator)
                .AsQueryable();

            if (typeId.HasValue)
                query = query.Where(v => v.Report.ViolationTypeId == typeId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(v => v.Status == status);

            return query.ToList();
        }
    }
}
