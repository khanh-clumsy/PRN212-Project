using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
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

        public async Task CreateReportWithAttachment(Report report, string filePath)
        {
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync(); 

            if (!string.IsNullOrEmpty(filePath))
            {
                // Kiểm tra phần mở rộng
                string ext = Path.GetExtension(filePath).ToLower();
                var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".mp4" };

                if (!allowedExts.Contains(ext))
                    MessageBox.Show("Chỉ hỗ trợ ảnh (.jpg, .jpeg, .png) và video (.mp4)");
                
                // Kiểm tra dung lượng file
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 5 * 1024 * 1024) // > 5MB
                    MessageBox.Show("File vượt quá kích thước cho phép (tối đa 5MB)");

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(filePath);
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
