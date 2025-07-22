using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;


namespace TrafficViolationFeedbackSystem.Views.Admin
{
    /// <summary>
    /// Interaction logic for AddViolationTypeView.xaml
    /// </summary>
    public partial class AddViolationTypeView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly Action _onCancel;

        public AddViolationTypeView(Action onCancel)
        {
            InitializeComponent();       
            _context = new TrafficViolationFeedbackSystemContext();
            _onCancel = onCancel;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu bắt buộc
                if (string.IsNullOrWhiteSpace(txtName.Text) ||
                    string.IsNullOrWhiteSpace(txtDescription.Text) ||
                    string.IsNullOrWhiteSpace(txtStandardFine.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin bắt buộc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate Mức phạt chuẩn: Phải là số thập phân không âm
                if (!decimal.TryParse(txtStandardFine.Text, out decimal standardFine) || standardFine < 0)
                {
                    MessageBox.Show("Mức phạt chuẩn phải là một số thập phân >0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra tên vi phạm đã tồn tại
                if (_context.ViolationTypes.Any(vt => vt.Name.ToLower() == txtName.Text.ToLower())) // Sửa từ ViolationTypes thành ViolationType
                {
                    MessageBox.Show("Tên vi phạm đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Thêm loại vi phạm mới
                var newViolationType = new ViolationType
                {
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    StandardFine = standardFine
                };

                _context.ViolationTypes.Add(newViolationType); 
                _context.SaveChanges();
                MessageBox.Show("Thêm loại vi phạm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                _onCancel();
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
