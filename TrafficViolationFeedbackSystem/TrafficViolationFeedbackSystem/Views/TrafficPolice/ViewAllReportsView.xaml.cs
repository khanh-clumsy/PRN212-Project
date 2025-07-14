using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TrafficViolationFeedbackSystem.Controllers;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.ViewModels;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class ViewAllReportsView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly ReportController _reportController;

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
            }
        }

        private async void btnReject_Click(object sender, RoutedEventArgs e)
        {
            if (dgReports.SelectedItem is ReportViewModel selected)
            {
                await _reportController.UpdateReportStatusAsync(selected.ReportId, "Rejected");
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
    }
}
