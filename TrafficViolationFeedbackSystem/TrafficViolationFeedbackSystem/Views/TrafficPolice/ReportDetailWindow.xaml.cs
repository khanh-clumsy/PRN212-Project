using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;
using TrafficViolationFeedbackSystem.Views.User;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class ReportDetailWindow : Window
    {
        private readonly TrafficViolationFeedbackSystemContext _context = new TrafficViolationFeedbackSystemContext();
        private readonly int _reportId;
        private Attachment _firstAttachment;
        public ReportDetailWindow(int reportId)
        {
            InitializeComponent();
            _reportId = reportId;
            LoadDetail();
        }
        private void LoadDetail()
        {
            var attachments = _context.Attachments
                .Where(a => a.ReportId == _reportId)
                .ToList();

            if (attachments.Any())
            {
                _firstAttachment = attachments.First(); // lưu lại để dùng
                txtAttachmentName.Text = attachments.First().FilePath;
            }
            else
            {
                txtAttachmentName.Text = "Không có file đính kèm";
                btnViewSingleAttachment.IsEnabled = false;
            }

            var report = _context.Reports.Include(r => r.ViolationType).FirstOrDefault(r => r.ReportId == _reportId);
            if (report == null) { this.Close(); return; }
            txtReportId.Text = $"Mã phản ánh: {report.ReportId}";
            txtPlateNumber.Text = $"Biển số: {report.PlateNumber}";
            txtViolationType.Text = $"Loại vi phạm: {report.ViolationType?.Name ?? ""}";
            txtDescription.Text = $"Mô tả: {report.Description}";
            txtLocation.Text = $"Địa điểm: {report.Location}";
            txtReportDate.Text = $"Ngày gửi: {report.ReportDate:dd/MM/yyyy HH:mm}";
            txtStatus.Text = $"Trạng thái: {report.Status switch
            {
                "Pending" => "Chờ duyệt",
                "Approved" => "Đã duyệt",
                "Rejected" => "Từ chối",
                _ => "Không xác định"
            }}";

        }
        public void btnViewSingleAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (_firstAttachment != null && !string.IsNullOrEmpty(_firstAttachment.FilePath))
            {
                string fullPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Image", "Report", _firstAttachment.FilePath);

                if (!File.Exists(fullPath))
                {
                    MessageBox.Show("Tệp không tồn tại: " + fullPath, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var preview = new AttachmentPreviewWindow(fullPath);
                preview.ShowDialog();
            }
        }
    }
}