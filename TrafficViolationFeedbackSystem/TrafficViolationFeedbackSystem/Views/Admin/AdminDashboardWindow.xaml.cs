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
using TrafficViolationFeedbackSystem.Views.Authentication;

namespace TrafficViolationFeedbackSystem.Views.Admin
{
    public partial class AdminDashboardWindow : Window
    {
        public readonly TrafficViolationFeedbackSystemContext _context;
        public AdminDashboardWindow()
        {
            InitializeComponent();
            _context = new TrafficViolationFeedbackSystemContext();
        }

        private void btnManageAccounts_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ManageAccountsView(userControl => MainContent.Content = userControl);
        }

        private void btnViolationStatistics_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ViolationStatisticsView(userControl => MainContent.Content = userControl);
        }

        private void btnViolationTypes_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ManageViolationTypesView(userControl => MainContent.Content = userControl);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}
