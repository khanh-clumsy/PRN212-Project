using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.Views.Authentication
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public Action ShowRegisterView { get; set; }
        public Action ShowForgotPasswordView { get; set; }
        public Action<Models.User> OnLoginSuccess { get; set; }

        private readonly UserController _userController;

        private readonly TrafficViolationFeedbackSystemContext _context;

        public LoginView(TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
            _userController = new UserController(_context);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy thông tin từ UI
                string email = txtEmail.Text?.Trim();
                string password = txtPassword.Password;

                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ email và mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Thực hiện đăng nhập
                var (success, message, user) = _userController.Login(email, password);

                if (success)
                {
                    MessageBox.Show(message, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), 
                        new Claim(ClaimTypes.Role, user.Role)                     
                    };

                    var identity = new ClaimsIdentity(claims, "Custom");
                    var principal = new ClaimsPrincipal(identity);

                    // Lưu vào ứng dụng
                    App.Current.Properties["CurrentUser"] = principal;

                    // Gọi callback khi đăng nhập thành công
                    OnLoginSuccess?.Invoke(user);

                    // Xóa dữ liệu trong form
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(message, "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tblGoToRegister_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ShowRegisterView?.Invoke();
        }

        private void tblForgotPassword_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ShowForgotPasswordView?.Invoke();
        }

        /// <summary>
        /// Xóa dữ liệu trong form
        /// </summary>
        private void ClearForm()
        {
            txtEmail.Text = string.Empty;
            txtPassword.Password = string.Empty;
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }
    }
}
