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
using System.Windows.Shapes;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Services;

namespace TrafficViolationFeedbackSystem.Views.Shared
{
    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        public ChangePasswordWindow()
        {
            InitializeComponent();
            _context = new TrafficViolationFeedbackSystemContext();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string currentPwd = txtCurrentPassword.Password;
            string newPwd = txtNewPassword.Password;
            string confirmPwd = txtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(currentPwd) ||
                string.IsNullOrWhiteSpace(newPwd) ||
                string.IsNullOrWhiteSpace(confirmPwd))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string userId = AuthenticationContext.UserId; 
            var user = _context.Users.FirstOrDefault(u => u.UserId == Int32.Parse(userId));
            // TODO: So sánh currentPwd với mật khẩu thật trong DB nếu cần
            if (HashPassword(currentPwd) != user.Password)
            {
                MessageBox.Show("Mật khẩu hiện tại không chính xác!", "Sai mật khẩu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (newPwd.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải từ 6 ký tự trở lên!", "Yêu cầu bảo mật", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPwd != confirmPwd)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi xác nhận", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            user.Password = HashPassword(newPwd);
            _context.SaveChanges();
            MessageBox.Show("Đổi mật khẩu thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
