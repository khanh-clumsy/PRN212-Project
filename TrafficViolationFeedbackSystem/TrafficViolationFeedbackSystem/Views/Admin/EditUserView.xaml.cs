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
    /// Interaction logic for EditUserView.xaml
    /// </summary>
    public partial class EditUserView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly int _userId;
        private readonly Action _onCancel;
        public EditUserView(int userId, Action onCancel)
        {
            InitializeComponent();
            _context = new TrafficViolationFeedbackSystemContext();
            _userId = userId;
            _onCancel = onCancel;
            LoadUserData();
        }

        private void LoadUserData()
        {
            var user = _context.Users.Find(_userId);
            if (user != null)
            {
                txtFullName.Text = user.FullName;
                txtEmail.Text = user.Email;
                txtPassword.Text = user.Password;
                cbRole.SelectedItem = cbRole.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == user.Role);
                txtPhone.Text = user.Phone;
                txtAddress.Text = user.Address;
            }
            else
            {
                MessageBox.Show("Không tìm thấy tài khoản!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                _onCancel();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

                // Kiểm tra email đã tồn tại (ngoại trừ email hiện tại của người dùng)
                var currentUser = _context.Users.Find(_userId);
                if (currentUser != null && _context.Users.Any(u => u.Email == txtEmail.Text && u.UserId != _userId))
                {
                    MessageBox.Show("Email đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                // Validate Số điện thoại: 10 số, bắt đầu bằng 0 (nếu có nhập)
                if (!string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    if (!Regex.IsMatch(txtPhone.Text, @"^0\d{9}$"))
                    {
                        MessageBox.Show("Số điện thoại phải gồm đúng 10 số và bắt đầu bằng số 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                // Kiểm tra số điện thoại đã tồn tại (ngoại trừ số hiện tại của người dùng)
                if (currentUser != null && !string.IsNullOrWhiteSpace(txtPhone.Text) && _context.Users.Any(u => u.Phone == txtPhone.Text && u.UserId != _userId))
                {
                    MessageBox.Show("Số điện thoại đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật thông tin người dùng
                var user = _context.Users.Find(_userId);
                if (user != null)
                {
                    user.FullName = txtFullName.Text;
                    user.Email = txtEmail.Text;
                    user.Password = txtPassword.Text;
                    user.Role = (cbRole.SelectedItem as ComboBoxItem)?.Content.ToString();
                    user.Phone = txtPhone.Text;
                    user.Address = txtAddress.Text;

                    _context.SaveChanges();
                    MessageBox.Show("Cập nhật tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    _onCancel(); // Quay lại màn hình quản lý tài khoản
                }
                else
                {
                    MessageBox.Show("Không tìm thấy tài khoản!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
