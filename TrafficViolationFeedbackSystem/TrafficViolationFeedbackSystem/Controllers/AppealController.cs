using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.ViewModels;

namespace TrafficViolationFeedbackSystem.Controllers
{
    public class AppealController
    {
        private readonly AppealDAO _appealDAO;
        private readonly TrafficViolationFeedbackSystemContext _context;
        public AppealController(TrafficViolationFeedbackSystemContext context)
        {
            _context = context;
            _appealDAO = new AppealDAO();
        }

        public List<AppealViewModel> GetAllAppeals()
        {
            using var newContext = new TrafficViolationFeedbackSystemContext(); 
            var appeals = newContext.Appeals
                .Include(a => a.Violation).ThenInclude(v => v.Report)
                .Include(a => a.Violator)
                .ToList();

            return appeals.Select(a => new AppealViewModel
            {
                AppealId = a.AppealId,
                ViolationId = a.ViolationId,
                PlateNumber = a.Violation?.Report?.PlateNumber ?? "",
                ViolatorName = a.Violator?.FullName ?? "",
                Content = a.Content,
                SubmitDate = a.SubmitDate,
                Result = a.Result
            }).ToList();
        }

    }
} 