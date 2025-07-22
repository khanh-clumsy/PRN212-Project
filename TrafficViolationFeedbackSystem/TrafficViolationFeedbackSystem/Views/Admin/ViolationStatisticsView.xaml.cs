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
using TrafficViolationFeedbackSystem.Models;



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
            LoadViolationTypes();
            LoadViolations();
        }
        private void LoadViolationTypes()
        {
            try
            {
                // Thêm "Tất cả loại vi phạm" làm mục mặc định
                cbViolationType.Items.Clear(); // Xóa các mục cũ để tránh trùng lặp
                cbViolationType.Items.Add(new ComboBoxItem { Content = "Tất cả loại vi phạm", Tag = 0 });

                var violationTypes = _context.ViolationTypes
                    .Select(vt => new ComboBoxItem { Content = vt.Name, Tag = vt.ViolationTypeId })
                    .ToList();

                if (violationTypes.Any())
                {
                    foreach (var item in violationTypes)
                    {
                        cbViolationType.Items.Add(item); 
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy loại vi phạm trong cơ sở dữ liệu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                cbViolationType.SelectedIndex = 0; // Chọn "Tất cả loại vi phạm" mặc định
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải loại vi phạm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadViolations(string licensePlateFilter = null, int? violationTypeId = null)
        {
            try
            {
                var violations = _context.Violations
                    .Include(v => v.Report)
                    .ThenInclude(r => r.ViolationType)
                    .Include(v => v.Violator)
                    .AsQueryable();

                if (violationTypeId.HasValue && violationTypeId != 0)
                {
                    violations = violations.Where(v => v.Report.ViolationTypeId == violationTypeId);
                }

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
            int? violationTypeId = null;
            if (cbViolationType.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                violationTypeId = (int?)selectedItem.Tag;
            }          
                LoadViolations(licensePlateFilter, violationTypeId);

            var filteredViolations = _context.Violations
                .Include(v => v.Report)
                .ThenInclude(r => r.ViolationType)
                .Include(v => v.Violator)
                .Where(v => (!violationTypeId.HasValue || violationTypeId == 0 || v.Report.ViolationTypeId == violationTypeId) &&
                            (string.IsNullOrWhiteSpace(licensePlateFilter) || (v.Report.PlateNumber != null && v.Report.PlateNumber.Contains(licensePlateFilter))))
                .ToList();

            if (filteredViolations.Count == 0)
            {
                MessageBox.Show("Không tìm thấy vi phạm với điều kiện đã nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void cbViolationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cbViolationType.SelectedItem is ComboBoxItem selectedItem)
            {
                int? violationTypeId = selectedItem.Tag as int?;
                LoadViolations(null, violationTypeId); // Load theo loại vi phạm, bỏ qua biển số

                // Kiểm tra nếu không có bản ghi sau khi load
                if (ViolationsGrid.Items.Count == 0 && violationTypeId != 0)
                {
                    MessageBox.Show("Không có vi phạm nào theo Loại vi phạm đã chọn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            LoadViolations(null, null);
            txtLicensePlateFilter.Text = string.Empty;
            cbViolationType.SelectedIndex = 0; // Quay lại "Tất cả loại vi phạm"
        }
        
        
    }
}
