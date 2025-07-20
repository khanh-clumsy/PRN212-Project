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

namespace TrafficViolationFeedbackSystem.Views.Admin
{
    /// <summary>
    /// Interaction logic for EditViolationTypeView.xaml
    /// </summary>
    public partial class EditViolationTypeView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly Action _onCancel;
        private readonly int _violationTypeId;
        public EditViolationTypeView(int violationTypeId, Action onCancel)
        {
            InitializeComponent();
            _context = new TrafficViolationFeedbackSystemContext();
            _onCancel = onCancel;
            _violationTypeId = violationTypeId;
            LoadViolationTypeData();
        }
        private void LoadViolationTypeData()
        {
            var violationType = _context.ViolationTypes.Find(_violationTypeId);
            if (violationType != null)
            {
                txtName.Text = violationType.Name;
                txtDescription.Text = violationType.Description ?? string.Empty;
                txtStandardFine.Text = violationType.StandardFine.ToString();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu bắt buộc
                if (string.IsNullOrWhiteSpace(txtName.Text) ||
                    string.IsNullOrWhiteSpace(txtStandardFine.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin bắt buộc (Tên vi phạm và Mức phạt chuẩn)!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate Mức phạt chuẩn: Phải là số thập phân không âm
                if (!decimal.TryParse(txtStandardFine.Text, out decimal standardFine) || standardFine < 0)
                {
                    MessageBox.Show("Mức phạt chuẩn phải là một số thập phân không âm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra tên vi phạm đã tồn tại (trừ bản ghi hiện tại)
                var existingViolationType = _context.ViolationTypes.FirstOrDefault(vt => vt.ViolationTypeId != _violationTypeId && vt.Name.ToLower() == txtName.Text.ToLower());
                if (existingViolationType != null)
                {
                    MessageBox.Show("Tên vi phạm đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật loại vi phạm
                var violationType = _context.ViolationTypes.Find(_violationTypeId);
                if (violationType != null)
                {
                    violationType.Name = txtName.Text;
                    violationType.Description = txtDescription.Text;
                    violationType.StandardFine = standardFine;
                    _context.SaveChanges();
                    MessageBox.Show("Cập nhật loại vi phạm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    _onCancel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _onCancel();
        }
    }
}
