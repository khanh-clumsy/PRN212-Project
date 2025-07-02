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

namespace TrafficViolationFeedbackSystem.Views.Authentication
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public Action ShowRegisterView { get; set; }
        public Action ShowForgotPasswordView { get; set; }

        public LoginView()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Gọi DAO đăng nhập ở đây
        }

        private void tblGoToRegister_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ShowRegisterView?.Invoke();
        }

        private void tblForgotPassword_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ShowForgotPasswordView?.Invoke();
        }

    }

}
