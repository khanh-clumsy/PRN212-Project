using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TrafficViolationFeedbackSystem.Controllers;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Services;

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

        private async Task LoadDataAsync()
        {
            string search = txtSearch.Text?.Trim().ToLower();
            var appeals = await Task.Run(() => _appealController.GetAllAppeals());
            if (!string.IsNullOrWhiteSpace(search))
            {
                appeals = appeals.Where(a =>
                    (a.PlateNumber != null && a.PlateNumber.ToLower().Contains(search))
                ).ToList();
            }
            dgAppeals.ItemsSource = appeals;
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (dgAppeals.SelectedItem is AppealViewModel selected)
            {
                var appeal = _context.Appeals
                    .Include(a => a.Violation)
                        .ThenInclude(v => v.Report)
                    .Include(a => a.Violator)
                    .FirstOrDefault(a => a.AppealId == selected.AppealId);
                if (appeal == null || appeal.Violation == null) return;
                var violation = appeal.Violation;
                var violator = appeal.Violator;
                var reporter = violation.Report?.ReporterId != null ? _context.Users.FirstOrDefault(u => u.UserId == violation.Report.ReporterId) : null;
                appeal.Result = "Approved";
                violation.IsCancelled = true;
                await _context.SaveChangesAsync();
                // Gửi email cho người bị báo cáo (người vi phạm)
                _ = Task.Run(async () =>
                {
                    if (violator != null && !string.IsNullOrEmpty(violator.Email))
                    {
                        string subject = "Đơn kháng cáo của bạn đã được duyệt";
                        string body = $"Chào {violator.FullName},\n\nĐơn kháng cáo của bạn về phương tiện {violation.Report.PlateNumber} đã được duyệt thành công. Vi phạm đã được hủy bỏ.";
                        await new EmailService().SendEmailAsync(violator.Email, subject, body);
                    }
                    // Gửi email cho người báo cáo
                    if (reporter != null && !string.IsNullOrEmpty(reporter.Email))
                    {
                        string subject = "Báo cáo của bạn đã bị kháng cáo thành công";
                        string body = $"Chào {reporter.FullName},\n\nBáo cáo về phương tiện {violation.Report.PlateNumber} đã bị kháng cáo thành công và vi phạm đã được hủy bỏ.";
                        await new EmailService().SendEmailAsync(reporter.Email, subject, body);
                    }
                });

                MessageBox.Show("Đã duyệt đơn kháng cáo và gửi email thông báo!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadDataAsync();
            }
        }

        private async void btnReject_Click(object sender, RoutedEventArgs e)
        {
            if (dgAppeals.SelectedItem is AppealViewModel selected)
            {
                var appeal = _context.Appeals
                    .Include(a => a.Violation)
                        .ThenInclude(v => v.Report)
                    .Include(a => a.Violator)
                    .FirstOrDefault(a => a.AppealId == selected.AppealId);
                if (appeal == null) return;
                var violation = appeal.Violation;
                var violator = appeal.Violator;
                appeal.Result = "Rejected";
                await _context.SaveChangesAsync();
                // Gửi email cho người vi phạm (bị từ chối kháng cáo)
                _ = Task.Run(async () =>
                {
                    if (violator != null && !string.IsNullOrEmpty(violator.Email))
                    {
                        string subject = "Đơn kháng cáo của bạn đã bị từ chối";
                        string body = $"Chào {violator.FullName},\n\nĐơn kháng cáo về phương tiện {violation.Report.PlateNumber} đã bị từ chối. Vui lòng liên hệ cơ quan chức năng để biết thêm chi tiết.";
                        await new EmailService().SendEmailAsync(violator.Email, subject, body);
                    }
                });
                MessageBox.Show("Đã từ chối đơn kháng cáo và gửi email thông báo!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadDataAsync();
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