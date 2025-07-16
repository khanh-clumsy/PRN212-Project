using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TrafficViolationFeedbackSystem.Controllers;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.ViewModels;
using TrafficViolationFeedbackSystem.Services;
using TrafficViolationFeedbackSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class ViewAllReportsView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly ReportController _reportController;
        private readonly EmailService _emailService = new EmailService();

        public ViewAllReportsView(TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
            _reportController = new ReportController(_context);
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            string search = txtSearch.Text?.Trim();
            string status = (cbStatusFilter.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            var reports = await _reportController.GetFilteredReportsAsync(search, status);
            dgReports.ItemsSource = reports;
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (dgReports.SelectedItem is ReportViewModel selected)
            {
                await _reportController.UpdateReportStatusAsync(selected.ReportId, "Approved");
                await LoadDataAsync();

                // Lấy thông tin report từ DB
                var report = await _context.Reports.Include(r => r.Reporter).FirstOrDefaultAsync(r => r.ReportId == selected.ReportId);
                if (report == null) return;
                report.ProcessedBy = Int32.Parse(AuthenticationContext.UserId);
                // Lấy violation (nếu có)
                var violation = await _context.Violations
                           .FirstOrDefaultAsync(v => v.ReportId == selected.ReportId);

                // Nếu chưa có thì tạo mới
                if (violation == null)
                {
                    // Tìm thông tin người vi phạm qua biển số
                    var vehicle = await _context.Vehicles
                        .Include(v => v.Owner)
                        .FirstOrDefaultAsync(v => v.PlateNumber == report.PlateNumber);

                    if (vehicle != null)
                    {
                        // Tìm mức phạt tiêu chuẩn từ loại vi phạm
                        var violationType = await _context.ViolationTypes
                            .FirstOrDefaultAsync(vt => vt.ViolationTypeId == report.ViolationTypeId);

                        decimal fine = violationType?.StandardFine ?? 0;

                        violation = new Violation
                        {
                            ReportId = report.ReportId,
                            ViolatorId = vehicle.Owner.UserId,
                            FineAmount = fine,
                            FineDate = DateTime.Now,
                        };
                        _context.Violations.Add(violation);
                        await _context.SaveChangesAsync();
                    }
                }
                // Gửi email cho người gửi báo cáo
                if (report?.Reporter != null && !string.IsNullOrEmpty(report.Reporter.Email))
                {
                    string subject = "Báo cáo của bạn đã được duyệt";
                    string body = $"Chào {report.Reporter.FullName},\n\nBáo cáo của bạn về phương tiện {report.PlateNumber} đã được duyệt thành công.\n\nCảm ơn bạn đã đóng góp cho cộng đồng!";
                    await _emailService.SendEmailAsync(report.Reporter.Email, subject, body);
                }

                // Gửi email cho người vi phạm
                if (violation?.Violator != null && !string.IsNullOrEmpty(violation.Violator.Email))
                {
                    string subject = "Bạn đã bị báo cáo vi phạm giao thông";
                    string body = $"Chào {violation.Violator.FullName},\n\nPhương tiện mang biển số {report?.PlateNumber} đã bị báo cáo vi phạm giao thông. Vui lòng kiểm tra chi tiết trong hệ thống hoặc liên hệ cơ quan chức năng để biết thêm thông tin.";
                    await _emailService.SendEmailAsync(violation.Violator.Email, subject, body);
                }
            }
        }

        private async void btnReject_Click(object sender, RoutedEventArgs e)
        {
            if (dgReports.SelectedItem is ReportViewModel selected)
            {
                await _reportController.UpdateReportStatusAsync(selected.ReportId, "Rejected");
                var report = await _context.Reports.Include(r => r.Reporter).FirstOrDefaultAsync(r => r.ReportId == selected.ReportId);
                if (report == null) return;
                report.ProcessedBy = Int32.Parse(AuthenticationContext.UserId); // Ghi nhận người xử lý
                await _context.SaveChangesAsync();
                await LoadDataAsync();
            }
        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            if (dgReports.SelectedItem is ReportViewModel selected)
            {
                var detailWindow = new ReportDetailWindow(selected.ReportId);
                detailWindow.ShowDialog();
            }
        }

        private void dgReports_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgReports.SelectedItem is ReportViewModel selected)
            {
                var detailWindow = new ReportDetailWindow(selected.ReportId);
                detailWindow.ShowDialog();
            }
        }
    }
}
