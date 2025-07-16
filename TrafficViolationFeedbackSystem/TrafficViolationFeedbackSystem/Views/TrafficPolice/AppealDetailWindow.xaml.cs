using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;
using TrafficViolationFeedbackSystem.Services;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class AppealDetailWindow : Window
    {
        private readonly TrafficViolationFeedbackSystemContext _context = new TrafficViolationFeedbackSystemContext();
        private readonly int _appealId;
        private Appeal _appeal;
        private Violation _violation;
        private Models.User _violator;
        private Models.User _reporter;
        private readonly EmailService _emailService = new EmailService();
        private Attachment _firstAttachment;

        public AppealDetailWindow(int appealId)
        {
            InitializeComponent();
            _appealId = appealId;
            LoadDetail();
        }

        private void LoadDetail()
        {
            _appeal = _context.Appeals
                .Include(a => a.Violation)
                    .ThenInclude(v => v.Report)
                .Include(a => a.Violator)
                .FirstOrDefault(a => a.AppealId == _appealId);
            if (_appeal == null) { this.Close(); return; }
            _violation = _appeal.Violation;
            _violator = _appeal.Violator;
            _reporter = _violation?.Report?.ReporterId != null ? _context.Users.FirstOrDefault(u => u.UserId == _violation.Report.ReporterId) : null;

            txtAppealId.Text = $"Mã đơn: {_appeal.AppealId}";
            txtPlateNumber.Text = $"Biển số: {_violation?.Report?.PlateNumber ?? ""}";
            txtViolatorName.Text = $"Người vi phạm: {_violator?.FullName ?? ""}";
            txtContent.Text = $"Nội dung: {_appeal.Content}";
            txtSubmitDate.Text = $"Ngày gửi: {_appeal.SubmitDate:dd/MM/yyyy HH:mm}";
            txtResult.Text = $"Kết quả: {_appeal.DisplayResult ?? "Chưa xử lý"}";

            // Hiển thị file đính kèm (ảnh/video)
            if (_violation?.Report != null)
            {
                var attachments = _context.Attachments.Where(a => a.ReportId == _violation.Report.ReportId).ToList();
                if (attachments.Any())
                {
                    _firstAttachment = attachments.First();
                    txtAttachmentName.Text = attachments.First().FilePath;
                    btnViewSingleAttachment.IsEnabled = true;
                }
                else
                {
                    txtAttachmentName.Text = "Không có file đính kèm";
                    btnViewSingleAttachment.IsEnabled = false;
                }
            }
            else
            {
                txtAttachmentName.Text = "Không có file đính kèm";
                btnViewSingleAttachment.IsEnabled = false;
            }
        }

        public void btnViewSingleAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (_firstAttachment != null && !string.IsNullOrEmpty(_firstAttachment.FilePath))
            {
                string fullPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "Image", "Report", _firstAttachment.FilePath);
                if (!System.IO.File.Exists(fullPath))
                {
                    MessageBox.Show("Tệp không tồn tại: " + fullPath, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var preview = new TrafficViolationFeedbackSystem.Views.User.AttachmentPreviewWindow(fullPath);
                preview.ShowDialog();
            }
        }
    }
} 