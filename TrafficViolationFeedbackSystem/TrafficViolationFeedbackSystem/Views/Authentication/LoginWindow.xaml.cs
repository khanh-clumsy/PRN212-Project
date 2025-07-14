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
using TrafficViolationFeedbackSystem.Views.Citizen;
using TrafficViolationFeedbackSystem.Views.TrafficPolice;

namespace TrafficViolationFeedbackSystem.Views.Authentication
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly TrafficViolationFeedbackSystemContext _context;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new TrafficViolationFeedbackSystemContext();

            ShowLoginView();
        }

        private void ShowLoginView()
        {
            var loginView = new LoginView(_context);
            loginView.ShowRegisterView = ShowRegisterView;
            loginView.ShowForgotPasswordView = ShowForgotPasswordView;
            loginView.OnLoginSuccess = OnLoginSuccess;
            MainContent.Content = loginView;
        }

        private void ShowRegisterView()
        {
            var registerView = new RegisterView(_context);
            registerView.ShowLoginView = ShowLoginView;
            MainContent.Content = registerView;
        }

        private void ShowForgotPasswordView()
        {
            var forgotPasswordView = new ForgotPasswordView(_context);
            forgotPasswordView.ShowLoginView = ShowLoginView;
            MainContent.Content = forgotPasswordView;
        }

        /// <summary>
        /// Xử lý sự kiện đăng nhập thành công
        /// </summary>
        /// <param name="user">Thông tin người dùng đã đăng nhập</param>
        private void OnLoginSuccess(Models.User user)
        {
            try
            {
                Window dashboardWindow = null;

                switch (user.Role)
                {
                    case "Citizen":
                        MessageBox.Show("Đăng nhập bằng vai trò: Người dân", "Vai trò", MessageBoxButton.OK);
                        dashboardWindow = new CitizenDashboardWindow();
                        break;

                    case "Admin":
                        MessageBox.Show("Admin", "Admin", MessageBoxButton.OK);
                        //dashboardWindow = new AdminDashboardWindow(); // nếu có
                        break;

                    case "TrafficPolice":
                        MessageBox.Show("Đăng nhập bằng vai trò: Cảnh sát giao thông", "Vai trò", MessageBoxButton.OK);
                        dashboardWindow = new TrafficPoliceDashboardWindow(); // nếu có
                        break;

                    default:
                        MessageBox.Show("Không xác định được vai trò người dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }
                dashboardWindow.Show();
                this.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi chuyển đến dashboard: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
