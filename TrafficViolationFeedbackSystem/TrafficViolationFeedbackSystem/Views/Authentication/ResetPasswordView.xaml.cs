using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Data;

namespace TrafficViolationFeedbackSystem.Views.Authentication
{
    /// <summary>
    /// Interaction logic for ResetPasswordView.xaml
    /// </summary>
    public partial class ResetPasswordView : UserControl
    {
        public Action ShowLoginView { get; set; }
        private readonly string _email;
        public Action OnResetPasswordSuccess { get; set; }
        private readonly TrafficViolationFeedbackSystemContext _context = new TrafficViolationFeedbackSystemContext();  
        public ResetPasswordView(string email)
        {
            InitializeComponent();
            this._email = email;
        }

        private async void btnReset_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự.", "Yêu cầu bảo mật", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == _email);
            if (user == null)
            {
                MessageBox.Show("Không tìm thấy người dùng tương ứng.", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Hash mật khẩu mới (SHA256 Base64)
            string hashedPassword = HashPassword(newPassword);
            user.Password = hashedPassword;

            // Xóa mã xác nhận
            user.ResetCode = null;
            user.ResetCodeExpiry = null;

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            MessageBox.Show("Mật khẩu của bạn đã được đặt lại thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            ShowLoginView?.Invoke(); // Quay lại trang đăng nhập
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        private void tblGoToLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowLoginView?.Invoke();
        }

        private void txtConfirmPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnReset_Click(sender, e);
            }
        }
    }
}
