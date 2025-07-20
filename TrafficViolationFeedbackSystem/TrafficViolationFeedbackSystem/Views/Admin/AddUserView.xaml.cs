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
using TrafficViolationFeedbackSystem.Data;

namespace TrafficViolationFeedbackSystem.Views.Admin
{
    /// <summary>
    /// Interaction logic for AddUserView.xaml
    /// </summary>
    public partial class AddUserView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly Action _onCancel;
        public AddUserView(Action onCancel)
        {
            InitializeComponent();
            _context = new TrafficViolationFeedbackSystemContext();
            _onCancel = onCancel;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Text) ||
                    cbRole.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin bắt buộc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                // Validate Họ và tên: Chỉ chứa chữ cái (bao gồm chữ có dấu) và dấu cách
                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    MessageBox.Show("Họ tên không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!Regex.IsMatch(txtFullName.Text, @"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚÝàáâãèéêìíòóôõùúýĂăĐđĨĩŨũƠơƯưẠ-ỹ\s]+$"))
                {
                    MessageBox.Show("Họ tên chỉ được chứa chữ cái và dấu cách, không chứa số hoặc ký tự đặc biệt!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate Email: Phải có đuôi @gmail.com
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Email không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!Regex.IsMatch(txtEmail.Text, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
                {
                    MessageBox.Show("Email phải có định dạng hợp lệ và kết thúc bằng @gmail.com!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate Mật khẩu: Ít nhất 6 ký tự
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Mật khẩu không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (txtPassword.Text.Length < 6)
                {
                    MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate Vai trò: Phải được chọn
                if (cbRole.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn vai trò!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate Số điện thoại: 10 số, bắt đầu bằng 0
                if (!string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    if (!Regex.IsMatch(txtPhone.Text, @"^0\d{9}$"))
                    {
                        MessageBox.Show("Số điện thoại phải gồm đúng 10 số và bắt đầu bằng số 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Kiểm tra email đã tồn tại trong cơ sở dữ liệu
                if (_context.Users.Any(u => u.Email == txtEmail.Text))
                {
                    MessageBox.Show("Email đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Kiểm tra Phone đã tồn tại trong cơ sở dữ liệu
                if (_context.Users.Any(u => u.Phone == txtPhone.Text))
                {
                    MessageBox.Show("Phone đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                var newUser = new Models.User
                {
                    FullName = txtFullName.Text,
                    Email = txtEmail.Text,
                    Password = txtPassword.Text,
                    Role = (cbRole.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Phone = txtPhone.Text,
                    Address = txtAddress.Text
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();
                MessageBox.Show("Thêm tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                _onCancel(); // Quay lại màn hình quản lý tài khoản
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
