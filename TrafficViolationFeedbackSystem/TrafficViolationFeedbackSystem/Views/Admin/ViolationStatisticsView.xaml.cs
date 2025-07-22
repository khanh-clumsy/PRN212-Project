using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrafficViolationFeedbackSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace TrafficViolationFeedbackSystem.Views.Admin
{
    /// <summary>
    /// Interaction logic for ViolationStatisticsView.xaml
    /// </summary>
    public partial class ViolationStatisticsView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly Action<UserControl> _setContent;
        public ViolationStatisticsView(Action<UserControl> setContent)
        {
            InitializeComponent();
            _context = new TrafficViolationFeedbackSystemContext();
            _setContent = setContent;
            LoadViolations();
        }
        private void LoadViolations(string licensePlateFilter = null)
        {
            try
            {
                var violations = _context.Violations
                    .Include(v => v.Report)
                    .ThenInclude(r => r.ViolationType)
                    .Include(v => v.Violator)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(licensePlateFilter))
                {
                    violations = violations.Where(v => v.Report.PlateNumber != null && v.Report.PlateNumber.Contains(licensePlateFilter));
                }

                ViolationsGrid.ItemsSource = violations.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thống kê vi phạm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            string licensePlateFilter = txtLicensePlateFilter.Text;
            if (string.IsNullOrWhiteSpace(licensePlateFilter))
            {
                MessageBox.Show("Vui lòng nhập biển số để lọc!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            LoadViolations(licensePlateFilter);

            // Kiểm tra nếu không có kết quả sau khi lọc
            var filteredViolations = _context.Violations
                .Include(v => v.Report)
                .Include(v => v.Violator)
                .Where(v => v.Report.PlateNumber != null && v.Report.PlateNumber.Contains(licensePlateFilter))
                .ToList();

            if (filteredViolations.Count == 0)
            {
                MessageBox.Show("Không tìm thấy vi phạm với biển số đã nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadViolations(licensePlateFilter);
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            LoadViolations(null); 
            txtLicensePlateFilter.Text = string.Empty;
        }

       
    }
}
