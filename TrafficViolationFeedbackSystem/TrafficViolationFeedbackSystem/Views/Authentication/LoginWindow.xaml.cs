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

namespace TrafficViolationFeedbackSystem.Views.Authentication
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            ShowLoginView();
        }

        private void ShowLoginView()
        {
            var loginView = new LoginView();
            loginView.ShowRegisterView = ShowRegisterView;
            loginView.ShowForgotPasswordView = ShowForgotPasswordView; 
            MainContent.Content = loginView;
        }

        private void ShowRegisterView()
        {
            var registerView = new RegisterView();
            registerView.ShowLoginView = ShowLoginView;
            MainContent.Content = registerView;
        }
        private void ShowForgotPasswordView()
        {
            var forgotPasswordView = new ForgotPasswordView();
            forgotPasswordView.ShowLoginView = ShowLoginView;
            MainContent.Content = forgotPasswordView;
        }
    }

}
