using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.DAO
{
    public class ReportDAO
    {
        private readonly TrafficViolationFeedbackSystemContext _context;

        public ReportDAO(TrafficViolationFeedbackSystemContext context)
        {
            _context = context;
        }
        public async Task UpdateStatus(int reportId, string newStatus)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(r => r.ReportId == reportId);
            if (report != null)
            {
                report.Status = newStatus;
                _context.SaveChanges();
            }
        }
        public async Task<List<Report>> GetAllReportsAsync()
        {
            return await _context.Reports
                 .Include(r => r.Reporter)
                 .Include(r => r.ViolationType)
                 .Include(r => r.Attachments)
                 .ToListAsync();
        }
        public async Task UpdateReportStatusAsync(int reportId, string status)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(r => r.ReportId == reportId);
            if (report != null)
            {
                report.Status = status;
                await _context.SaveChangesAsync();
            }
        }
        public async Task CreateReportWithAttachment(Report report, string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string ext = Path.GetExtension(filePath).ToLower();
                var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".mp4" };

                if (!allowedExts.Contains(ext))
                {
                    throw new InvalidOperationException("Chỉ hỗ trợ ảnh (.jpg, .jpeg, .png) và video (.mp4).");
                }

                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 5 * 1024 * 1024)
                {
                    throw new InvalidOperationException("File vượt quá kích thước cho phép (tối đa 5MB).");
                }
            }

            // Lưu báo cáo
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(filePath))
            {
                string ext = Path.GetExtension(filePath).ToLower();
                string fileName = Guid.NewGuid().ToString() + ext;
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Image", "Report");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string destinationPath = Path.Combine(folderPath, fileName);
                File.Copy(filePath, destinationPath, true);

                string fileType = (ext == ".mp4") ? "Video" : "Image";

                var attachment = new Attachment
                {
                    ReportId = report.ReportId,
                    FilePath = fileName,
                    FileType = fileType
                };

                await _context.Attachments.AddAsync(attachment);
                await _context.SaveChangesAsync();
            }
        }


    }
}
