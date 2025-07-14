using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;
using TrafficViolationFeedbackSystem.ViewModels;

namespace TrafficViolationFeedbackSystem.Controllers
{
    public class ReportController
    {
        private readonly ReportDAO _reportDAO;
        private readonly ViolationTypesDAO _violationTypesDAO;
        private readonly UserDAO _userDAO;


        public ReportController(TrafficViolationFeedbackSystemContext context)
        {
            _reportDAO = new ReportDAO(context);
            _violationTypesDAO = new ViolationTypesDAO(context);
            _userDAO = new UserDAO(context);
        }

        public async Task SubmitReport(Report report, string attachmentFilePath)
        {
            await _reportDAO.CreateReportWithAttachment(report, attachmentFilePath);
        }
        public async Task UpdateReportStatusAsync(int reportId, string newStatus)
        {
            await _reportDAO.UpdateReportStatusAsync(reportId, newStatus);
        }

        public async Task<List<ReportViewModel>> GetFilteredReportsAsync(string search, string status)
        {
            var violationTypes = _violationTypesDAO.GetAllViolationTypes();
            var reports = await _reportDAO.GetAllReportsAsync();
            var query = reports.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(r =>
                    r.PlateNumber.ToLower().Contains(search)
                );
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r => r.Status == status);
            }

            return query
                .ToList()
                .Select(r => new ReportViewModel
                {
                    ReportId = r.ReportId,
                    PlateNumber = r.PlateNumber,
                    OwnerName = _userDAO.GetVehicleOwnerNameByPlateNumber(r.PlateNumber),
                    ViolationTypeName = violationTypes.FirstOrDefault(v => v.ViolationTypeId == r.ViolationTypeId)?.Name ?? "",
                    Description = r.Description,
                    Location = r.Location,
                    ReportDate = r.ReportDate,
                    Status = r.Status,
                }).ToList();
        }
    }
}
