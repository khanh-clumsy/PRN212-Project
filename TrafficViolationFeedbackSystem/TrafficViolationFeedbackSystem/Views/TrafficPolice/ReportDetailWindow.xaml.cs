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
        public ReportDetailWindow(int reportId)
        {
            InitializeComponent();
            _reportId = reportId;
            LoadDetail();
        }
        private void LoadDetail()
        {
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
            var attachments = _context.Attachments.Where(a => a.ReportId == _reportId).ToList();
            lstAttachments.ItemsSource = attachments;
        }
        private void btnViewAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (lstAttachments.SelectedItem is Attachment att && !string.IsNullOrEmpty(att.FilePath))
            {
                var previewWindow = new AttachmentPreviewWindow(att.FilePath);
                previewWindow.ShowDialog();
            }
        }
    }
} 