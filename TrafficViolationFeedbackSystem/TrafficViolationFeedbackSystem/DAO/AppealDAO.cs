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
    public class AppealDAO
    {
        private readonly TrafficViolationFeedbackSystemContext _context;

        public AppealDAO()
        {
            _context = new TrafficViolationFeedbackSystemContext();
        }
        public bool AddAppeal(Appeal appeal)
        {
            try
            {
                _context.Appeals.Add(appeal);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Có thể log lỗi ở đây nếu cần
                return false;
            }
        }
        public bool AppealExists(int violationId)
        {
            return _context.Appeals.Any(a => a.ViolationId == violationId);
        }

        public List<Appeal> GetAppealsByUser(int userId)
        {
            return _context.Appeals
                .Include(a => a.Violation)
                    .ThenInclude(v => v.Report)
                        .ThenInclude(r => r.ViolationType)
                .Where(a => a.ViolatorId == userId)
                .OrderByDescending(a => a.SubmitDate)
                .ToList();
        }

        public string GetViolationSummary(int violationId)
        {
            var result = _context.Violations
                .Where(v => v.ViolationId == violationId)
                .Select(v => new
                {
                    TypeName = v.Report.ViolationType.Name,
                    Date = v.FineDate ?? v.Report.ReportDate
                })
                .FirstOrDefault();

            if (result != null)
            {
                return $"{result.TypeName} - {result.Date:dd/MM/yyyy}";
            }

            return "Không tìm thấy vi phạm";
        }

        public Appeal GetAppealById(int id)
        {
            return _context.Appeals.Include(a => a.Violation).ThenInclude(v => v.Report).ThenInclude(r => r.ViolationType).FirstOrDefault(a => a.AppealId == id);
        }

        public bool DeleteAppeal(int id)
        {
            var appeal = _context.Appeals.Find(id);
            if (appeal == null || !string.IsNullOrEmpty(appeal.Result))
                return false;

            _context.Appeals.Remove(appeal);
            _context.SaveChanges();
            return true;
        }
        public bool UpdateAppeal(int appealId, string newContent)
        {
            var appeal = _context.Appeals.Find(appealId);
            if (appeal == null) return false;

            appeal.Content = newContent;
            _context.SaveChanges();
            return true;
        }

    }
}
