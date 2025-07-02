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

namespace TrafficViolationFeedbackSystem.Views.Citizen
{
    /// <summary>
    /// Interaction logic for CitizenDashboardWindow.xaml
    /// </summary>
    public partial class CitizenDashboardWindow : Window
    {
        public CitizenDashboardWindow()
        {
            InitializeComponent();
            MainContent.Content = new HomeView(SetContent);
        }
        public void SetContent(UserControl view)
        {
            MainContent.Content = view;
        }
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            SetContent(new HomeView(SetContent));
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Xử lý mở Hồ sơ cá nhân
            SetContent(new ProfileView());
        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Xử lý xem Thống kê
            SetContent(new StatisticsView());
        }

        
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
