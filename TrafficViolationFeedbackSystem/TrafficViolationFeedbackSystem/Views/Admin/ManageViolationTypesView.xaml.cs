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
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.Views.Admin
{
    public partial class ManageViolationTypesView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly Action<UserControl> _setContent;
        public ManageViolationTypesView(Action<UserControl> setContent)
        {
            InitializeComponent();
            _context = new TrafficViolationFeedbackSystemContext();
            _setContent = setContent;
            LoadViolationTypes();
        }

        private void LoadViolationTypes()
        {
            try
            {
                ViolationTypesGrid.ItemsSource = _context.ViolationTypes.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách loại vi phạm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddViolationType_Click(object sender, RoutedEventArgs e)
        {
            _setContent(new AddViolationTypeView(() => _setContent(new ManageViolationTypesView(_setContent))));
        }

        private void btnEditViolationType_Click(object sender, RoutedEventArgs e)
        {
            if (ViolationTypesGrid.SelectedItem is ViolationType selectedViolationType)
            {
                _setContent(new EditViolationTypeView(selectedViolationType.ViolationTypeId, () => _setContent(new ManageViolationTypesView(_setContent))));
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một loại vi phạm để sửa!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDeleteViolationType_Click(object sender, RoutedEventArgs e)
        {
            if (ViolationTypesGrid.SelectedItem is ViolationType selectedViolationType)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa loại vi phạm '{selectedViolationType.Name}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var violationTypeToDelete = _context.ViolationTypes.Find(selectedViolationType.ViolationTypeId);
                        if (violationTypeToDelete != null)
                        {
                            _context.ViolationTypes.Remove(violationTypeToDelete);
                            _context.SaveChanges();
                            LoadViolationTypes(); // Làm mới danh sách
                            MessageBox.Show("Xóa loại vi phạm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa loại vi phạm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một loại vi phạm để xóa!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
