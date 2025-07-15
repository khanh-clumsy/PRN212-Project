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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrafficViolationFeedbackSystem.Controllers;
using TrafficViolationFeedbackSystem.Data;

namespace TrafficViolationFeedbackSystem.Views.Authentication
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public Action ShowLoginView { get; set; }

        private readonly UserController _userController;
        private readonly TrafficViolationFeedbackSystemContext _context;

        public RegisterView(TrafficViolationFeedbackSystemContext context)
        { 
            InitializeComponent();
            _context = context;
            _userController = new UserController(_context);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy thông tin từ UI
                string fullName = txtFullName.Text?.Trim();
                string email = txtEmail.Text?.Trim();
                string password = pwdPassword.Password;
                string confirmPassword = pwdConfirmPassword.Password;

                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || 
                    string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin bắt buộc.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra định dạng email
                if (!IsValidEmail(email))
                {
                    MessageBox.Show("Email không hợp lệ. Vui lòng nhập email đúng định dạng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Thực hiện đăng ký
                var (success, message) = _userController.Register(fullName, email, password, confirmPassword);

                if (success)
                {
                    MessageBox.Show(message, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Chuyển về trang đăng nhập
                    ShowLoginView?.Invoke();
                    
                    // Xóa dữ liệu trong form
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(message, "Lỗi đăng ký", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tblGoToLogin_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ShowLoginView?.Invoke();
        }

        /// <summary>
        /// Kiểm tra định dạng email
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <returns>True nếu email hợp lệ</returns>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Xóa dữ liệu trong form
        /// </summary>
        private void ClearForm()
        {
            txtFullName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            pwdPassword.Clear();
            pwdConfirmPassword.Clear();
        }

        /// <summary>
        /// Xử lý sự kiện nhấn Enter trong ô confirm password
        /// </summary>
        private void txtConfirmPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnRegister_Click(sender, e);
            }
        }
    }
}
