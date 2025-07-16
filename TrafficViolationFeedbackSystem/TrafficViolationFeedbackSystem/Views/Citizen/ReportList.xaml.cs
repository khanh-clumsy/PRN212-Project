using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TrafficViolationFeedbackSystem.Controllers;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Services;
using TrafficViolationFeedbackSystem.ViewModels;

namespace TrafficViolationFeedbackSystem.Views.Citizen
{
    public partial class ReportList : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context = new TrafficViolationFeedbackSystemContext();
        private readonly ReportController _reportController;
        public ReportList()
        {
            InitializeComponent();
            _reportController = new ReportController(_context);
            _ = LoadDataAsync();
        }
        private async Task LoadDataAsync()
        {
            int userId = int.Parse(AuthenticationContext.UserId);
            string search = txtSearch.Text?.Trim();
            string status = (cbStatusFilter.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var allReports = await _reportController.GetFilteredReportsAsyncByReporterID(userId, search, status);
            dgReports.ItemsSource = allReports;
        }
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }
        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            if (dgReports.SelectedItem is ReportViewModel selected)
            {
                var detailWindow = new TrafficPolice.ReportDetailWindow(selected.ReportId);
                detailWindow.ShowDialog();
            }
        }
        private void dgReports_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgReports.SelectedItem is ReportViewModel selected)
            {
                var detailWindow = new TrafficPolice.ReportDetailWindow(selected.ReportId);
                detailWindow.ShowDialog();
            }
        }
    }
}
