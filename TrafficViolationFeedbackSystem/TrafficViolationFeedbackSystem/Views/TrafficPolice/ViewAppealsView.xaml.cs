using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Controllers;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;
using TrafficViolationFeedbackSystem.Services;
using TrafficViolationFeedbackSystem.ViewModels;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class ViewAppealsView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly AppealController _appealController;

        public ViewAppealsView(TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
            _appealController = new AppealController(_context);
            _ = LoadDataAsync();
        }

        private async Task<List<AppealViewModel>> GetAppealsAsync(string search = null)
        {
            // Lấy lại danh sách appeals mới nhất từ DB
            var appeals = await Task.Run(() => _appealController.GetAllAppeals());
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                appeals = appeals.Where(a =>
                    (a.PlateNumber != null && a.PlateNumber.ToLower().Contains(search))
                ).ToList();
            }
            return appeals;
        }

        private async Task LoadDataAsync()
        {
            string search = txtSearch.Text?.Trim();
            var appeals = await GetAppealsAsync(search);
            dgAppeals.ItemsSource = null;
            dgAppeals.ItemsSource = appeals.ToList(); // luôn tạo list mới
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (dgAppeals.SelectedItem is AppealViewModel selected)
            {
                // Lấy lại entity từ DB
                var appeal = await _context.Appeals
                    .Include(a => a.Violation)
                        .ThenInclude(v => v.Report)
                    .Include(a => a.Violator)
                    .FirstOrDefaultAsync(a => a.AppealId == selected.AppealId);
                if (appeal == null || appeal.Violation == null) return;
                var violation = appeal.Violation;
                var violator = appeal.Violator;
                var reporter = violation.Report?.ReporterId != null ? _context.Users.FirstOrDefault(u => u.UserId == violation.Report.ReporterId) : null;
                appeal.Result = "Approved";
                violation.Status = "Cancelled";
                await _context.SaveChangesAsync();
                await LoadDataAsync();
                dgAppeals.SelectedItem = null;
                // Gửi email sau khi UI đã reload
                if (violator != null && !string.IsNullOrEmpty(violator.Email))
                {
                    string subject = "Đơn kháng cáo của bạn đã được duyệt";
                    string body = $"Chào {violator.FullName},\n\nĐơn kháng cáo của bạn về phương tiện {violation.Report.PlateNumber} đã được duyệt thành công. Vi phạm đã được hủy bỏ.";
                    await new EmailService().SendEmailAsync(violator.Email, subject, body);
                }
                if (reporter != null && !string.IsNullOrEmpty(reporter.Email))
                {
                    string subject = "Báo cáo của bạn đã bị kháng cáo thành công";
                    string body = $"Chào {reporter.FullName},\n\nBáo cáo về phương tiện {violation.Report.PlateNumber} đã bị kháng cáo thành công và vi phạm đã được hủy bỏ.";
                    await new EmailService().SendEmailAsync(reporter.Email, subject, body);
                }
            }
        }

        private async void btnReject_Click(object sender, RoutedEventArgs e)
        {
            if (dgAppeals.SelectedItem is AppealViewModel selected)
            {
                var appeal = await _context.Appeals
                    .Include(a => a.Violation)
                        .ThenInclude(v => v.Report)
                    .Include(a => a.Violator)
                    .FirstOrDefaultAsync(a => a.AppealId == selected.AppealId);
                if (appeal == null) return;
                var violation = appeal.Violation;
                var violator = appeal.Violator;
                appeal.Result = "Rejected";
                violation.Status = "RejectedAppeal";
                // Tạo fine mới nếu cần
                var v = await _context.Violations
                    .Include(vio => vio.Report)
                    .FirstOrDefaultAsync(vio => vio.ViolationId == violation.ViolationId);
                if (v != null)
                {
                    var fine = new Fine
                    {
                        ViolationId = v.ViolationId,
                        Amount = v.FineAmount,
                        Status = "Pending"
                    };
                    _context.Fines.Add(fine);
                }
                await _context.SaveChangesAsync();
                await LoadDataAsync();
                dgAppeals.SelectedItem = null;
                // Gửi email sau khi UI đã reload
                if (violator != null && !string.IsNullOrEmpty(violator.Email))
                {
                    string subject = "Đơn kháng cáo của bạn đã bị từ chối! Vui lòng vào ứng dụng để nộp phạt trực tuyến!";
                    string body = $"Chào {violator.FullName},\n\nĐơn kháng cáo về phương tiện {violation.Report.PlateNumber} đã bị từ chối. Vui lòng liên hệ cơ quan chức năng hoặc thanh toán tiền phạt online.";
                    await new EmailService().SendEmailAsync(violator.Email, subject, body);
                }
            }
        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            if (dgAppeals.SelectedItem is AppealViewModel selected)
            {
                var detailWindow = new AppealDetailWindow(selected.AppealId);
                detailWindow.ShowDialog();
            }
        }

        private void dgAppeals_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnDetail_Click(sender, e);
        }
    }
}