using System.Threading.Tasks;
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.Controllers
{
    public class ReportController
    {
        private readonly ReportDAO _reportDAO;

        public ReportController(TrafficViolationFeedbackSystemContext context)
        {
            _reportDAO = new ReportDAO(context);
        }

        public async Task SubmitReport(Report report, string attachmentFilePath)
        {
            await _reportDAO.CreateReportWithAttachment(report, attachmentFilePath);
        }
    }
}
